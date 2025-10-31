// Thêm "using" cho mọi thứ ta cần
using FlightAPI.Data; // Cho DbContext
using FlightAPI.Data.Entities; // Cho Entities (Booking, FlightInstance...)
using FlightAPI.Services.Dtos.Booking; // Cho DTOs
using Microsoft.EntityFrameworkCore; // Cho .Include() và .FirstOrDefaultAsync()

namespace FlightAPI.Services
{
    public class BookedService : IBookedService
    {
        private readonly AirCloudDbContext _context;

        // Yêu cầu "tiêm" (inject) cái DbContext vào
        public BookedService(AirCloudDbContext context)
        {
            _context = context;
        }

        public async Task<BookingResponseDto> CreateBookingAsync(BookingRequestDto dto, int accountId)
        {
            // === BƯỚC 1: KIỂM TRA ĐẦU VÀO ===

            // 1.1: Tìm chuyến bay
            var flight = await _context.FlightInstances
                .Include(fi => fi.DepartureAirportNavigation) // Lấy tên sân bay đi
                .Include(fi => fi.ArrivalAirportNavigation) // Lấy tên sân bay đến
                .FirstOrDefaultAsync(fi => fi.FlightInstanceId == dto.FlightInstanceId);

            if (flight == null)
            {
                throw new KeyNotFoundException("Chuyến bay không tồn tại.");
            }

            // 1.2: Tìm hạng ghế (để lấy hệ số nhân tiền)
            var seatClass = await _context.SeatClasses.FindAsync(dto.SeatClassId);
            if (seatClass == null)
            {
                throw new KeyNotFoundException("Hạng ghế không tồn tại.");
            }

            // === BƯỚC 2: KIỂM TRA LOGIC NGHIỆP VỤ (Quan trọng) ===

            int passengersCount = dto.Passengers.Count;

            // Đếm xem hạng ghế này còn bao nhiêu ghế TRỐNG
            int availableSeats = await _context.Seats.CountAsync(s =>
                s.FlightInstanceId == dto.FlightInstanceId &&
                s.SeatClassId == dto.SeatClassId &&
                s.IsAvailable == true);

            if (availableSeats < passengersCount)
            {
                throw new Exception($"Không đủ vé. Hạng {seatClass.Name} chỉ còn {availableSeats} vé.");
            }

            // === BƯỚC 3: TÍNH TOÁN ===

            // Tính giá vé cho 1 người (Giá gốc * Hệ số)
            decimal farePerPassenger = (decimal)(flight.BasePrice * seatClass.PriceMultiplier);

            // Tổng tiền (chưa tính dịch vụ, thuế, v.v. - ta làm sau)
            decimal totalAmount = farePerPassenger * passengersCount;

            // === BƯỚC 4: TẠO DỮ LIỆU "KÉT SẮT" (ENTITIES) ===

            // 4.1: Tạo "Đơn hàng" (Booking)
            var newBooking = new Booking
            {
                AccountId = accountId,
                TotalAmount = totalAmount,
                BookingStatus = "Pending", // Trạng thái "Chờ thanh toán"
                BookingCode = GenerateBookingCode(), // Tạo mã PNR
                CreatedAt = DateTime.UtcNow
            };

            // 4.2: Tạo "Chi tiết vé" cho từng hành khách (BookingFlight)
            foreach (var passengerDto in dto.Passengers)
            {
                newBooking.BookingFlights.Add(new BookingFlight
                {
                    FlightInstanceId = dto.FlightInstanceId,
                    PassengerName = passengerDto.PassengerName.ToUpper(), // Viết hoa tên
                    PassengerType = passengerDto.PassengerType,
                    Fare = farePerPassenger
                    // Lưu ý: Chúng ta KHÔNG chọn ghế (SeatId = null) ở bước này.
                    // Chọn ghế là một API (bước) riêng.
                });
            }

            // === BƯỚC 5: LƯU VÀO DATABASE ===

            // Thêm "đơn hàng mẹ" (nó sẽ tự động thêm "đơn hàng con" - BookingFlights)
            _context.Bookings.Add(newBooking);

            // Ghi tất cả thay đổi xuống DB
            await _context.SaveChangesAsync();

            // === BƯỚC 6: TRẢ "BIÊN NHẬN" (DTO) VỀ ===
            return new BookingResponseDto
            {
                BookingId = newBooking.BookingId,
                BookingCode = newBooking.BookingCode,
                TotalAmount = newBooking.TotalAmount,
                BookingStatus = newBooking.BookingStatus,
                CreatedAt = (DateTime)newBooking.CreatedAt,
                FlightInfo = $"{flight.DepartureAirportNavigation.AirportCode} -> {flight.ArrivalAirportNavigation.AirportCode}",

                // === PHẦN "VÁ" NẰM Ở ĐÂY ===
                BookingFlights = newBooking.BookingFlights.Select(bf => new BookingFlightIdDto
                {
                    BookingFlightId = bf.BookingFlightId,
                    PassengerName = bf.PassengerName
                }).ToList()
                // ============================
            };
        }

        public async Task SelectSeatAsync(SelectSeatRequestDto dto, int accountId)
        {
            // === BƯỚC 1: KIỂM TRA "HÀNG" (Entities) ===

            // 1.1: Tìm cái vé (của hành khách)
            // Phải "vớ" (Include) luôn "đơn hàng cha" (Booking) để check an ninh
            var bookingFlight = await _context.BookingFlights
                .Include(bf => bf.Booking)
                .FirstOrDefaultAsync(bf => bf.BookingFlightId == dto.BookingFlightId);

            if (bookingFlight == null)
            {
                throw new KeyNotFoundException("Vé (BookingFlight) không tồn tại.");
            }

            // 1.2: Tìm cái ghế (mà họ muốn chọn)
            var newSeat = await _context.Seats.FindAsync(dto.SeatId);
            if (newSeat == null)
            {
                throw new KeyNotFoundException("Ghế (Seat) không tồn tại.");
            }

            // === BƯỚC 2: KIỂM TRA "AN NINH" & "LOGIC" (Quan trọng) ===

            // 2.1: AN NINH 3 LỚP (Check "chủ")
            // Sếp có phải là "chủ" của cái đơn hàng này không?
            if (bookingFlight.Booking.AccountId != accountId)
            {
                throw new Exception("Bạn không có quyền sửa đơn hàng của người khác.");
            }

            // 2.2: LOGIC "VỢT VÉ"
            // Cái ghế này... còn "trống" (Available) không?
            if ((bool)!newSeat.IsAvailable)
            {
                throw new Exception($"Ghế {newSeat.SeatNumber} đã có người vợt mất. Vui lòng chọn ghế khác.");
            }

            // 2.3: LOGIC "GHÉP LỘN"
            // Cái ghế này có "khớp" với chuyến bay của sếp không?
            if (newSeat.FlightInstanceId != bookingFlight.FlightInstanceId)
            {
                throw new Exception("Lỗi logic: Bạn đang chọn ghế của một chuyến bay khác.");
            }

            // (Bỏ qua logic check Hạng ghế (Class) cho... đơn giản)

            // === BƯỚC 3: XỬ LÝ "ĐỔI GHẾ" (Nếu có) ===

            // 3.1: Check xem hành khách này đã "chọn" ghế nào trước đó chưa?
            if (bookingFlight.SeatId != null)
            {
                // A! Có. Phải "thả" (free up) cái ghế cũ ra
                var oldSeat = await _context.Seats.FindAsync(bookingFlight.SeatId);
                if (oldSeat != null)
                {
                    oldSeat.IsAvailable = true; // "Thả" ghế cũ
                }
            }

            // === BƯỚC 4: "KHÓA GHẾ" (Nghiệp vụ chính) ===

            // 4.1: "Khóa" cái ghế mới
            newSeat.IsAvailable = false;

            // 4.2: "Gắn" cái ghế mới vào "vé" của hành khách
            bookingFlight.SeatId = newSeat.SeatId;

            // 4.3: Lưu tất cả thay đổi (thả ghế cũ, khóa ghế mới, cập nhật vé)
            await _context.SaveChangesAsync();
        }

        // --- Hàm tiện ích private để tạo mã PNR ---
        private string GenerateBookingCode()
        {
            // Tạo 6 ký tự (chữ/số) ngẫu nhiên
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var result = new string(
                Enumerable.Repeat(chars, 6)
                          .Select(s => s[random.Next(s.Length)])
                          .ToArray());

            // TODO: (Nâng cao) Phải check xem code này đã tồn tại trong DB chưa
            return result;
        }
    }
}