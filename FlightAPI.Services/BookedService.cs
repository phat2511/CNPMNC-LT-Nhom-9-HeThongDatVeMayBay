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
                FlightInfo = $"{flight.DepartureAirportNavigation.AirportCode} -> {flight.ArrivalAirportNavigation.AirportCode}"
            };
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