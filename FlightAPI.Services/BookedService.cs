// Thêm "using" cho mọi thứ ta cần
using FlightAPI.Data; // Cho DbContext
using FlightAPI.Data.Entities; // Cho Entities (Booking, FlightInstance...)
using FlightAPI.Services.Dtos.Booking; // Cho DTOs
using FlightAPI.Services.Dtos.Payment;
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
            return await GetBookingResponseDtoById(newBooking.BookingId);
        }

        public async Task<PaymentResponseDto> ProcessPaymentAsync(int bookingId, PaymentRequestDto dto, int accountId)
        {
            // === BƯỚC 1: MỞ "LƯỚI AN TOÀN" (Transaction) ===
            // 'await using' đảm bảo dù thành công hay thất bại, nó đều tự dọn dẹp
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // === BƯỚC 2: KIỂM TRA ĐƠN HÀNG ===

                // 2.1: Tìm đơn hàng
                var booking = await _context.Bookings
                    .FirstOrDefaultAsync(b => b.BookingId == bookingId);

                if (booking == null)
                {
                    throw new KeyNotFoundException("Đơn hàng (Booking) không tồn tại.");
                }

                // 2.2: KIỂM TRA "AN NINH" (Check "chủ")
                if (booking.AccountId != accountId)
                {
                    throw new Exception("Bạn không có quyền thanh toán cho đơn hàng của người khác.");
                }

                // 2.3: KIỂM TRA LOGIC (Check "trạng thái")
                // Đơn này đã "Confirmed" (Thành công) hoặc "Cancelled" (Đã hủy) chưa?
                if (booking.BookingStatus != "Pending")
                {
                    throw new InvalidOperationException($"Không thể thanh toán. Đơn hàng đang ở trạng thái '{booking.BookingStatus}'.");
                }

                // === BƯỚC 3: TẠO "BIÊN LAI" (Payment Entity) ===
                // (Trong thực tế, sếp sẽ gọi cổng thanh toán ở đây. 
                //  Giờ ta "giả lập" là nó thành công luôn.)

                var newPayment = new Payment
                {
                    BookingId = bookingId,
                    Amount = booking.TotalAmount, // Lấy tiền từ đơn hàng
                    PaymentMethod = dto.PaymentMethod,
                    PaidAt = DateTime.UtcNow,
                    Status = "Success" // Giả lập là "Thành công"
                };

                _context.Payments.Add(newPayment);

                // === BƯỚC 4: "CHỐT SỔ" (Update Booking Entity) ===
                // "Đá" trạng thái đơn hàng
                booking.BookingStatus = "Confirmed";

                // === BƯỚC 5: LƯU & "CHỐT" TRANSACTION ===

                // 5.1: Lưu cả 2 thay đổi (Thêm Payment MỚI và Sửa Booking CŨ)
                await _context.SaveChangesAsync();

                // 5.2: "Chốt" Lưới An Toàn. Báo với DB: "OK, cho 2 thay đổi này 'sống'!"
                await transaction.CommitAsync();

                // === BƯỚC 6: TRẢ "BIÊN LAI XỊN" (DTO) VỀ ===
                return new PaymentResponseDto
                {
                    PaymentId = newPayment.PaymentId,
                    BookingId = newPayment.BookingId,
                    PaymentStatus = newPayment.Status,
                    BookingStatus = booking.BookingStatus, // Trạng thái mới: "Confirmed"
                    AmountPaid = (decimal)newPayment.Amount,
                    PaidAt = (DateTime)newPayment.PaidAt
                };
            }
            catch (Exception ex)
            {
                // === BƯỚC X (LỖI!): "QUAY TUA" (Rollback) ===
                // Báo với DB: "Hủy! Xóa sổ mọi thay đổi vừa làm!"
                await transaction.RollbackAsync();

                // Ném lỗi ra cho Controller bắt
                throw new Exception($"Thanh toán thất bại. {ex.Message}");
            }
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

        public async Task<IEnumerable<BookingHistoryDto>> GetMyBookingHistoryAsync(int accountId)
        {
            // === BƯỚC 1: "XUỐNG KHO" (VÀO BẢNG BOOKING) ===
            var bookings = await _context.Bookings
                // 1.1: Lọc: Chỉ lấy đơn của "tôi" (accountId)
                .Where(b => b.AccountId == accountId)

                // 1.2: Sắp xếp: Đơn mới nhất lên trên
                .OrderByDescending(b => b.CreatedAt)

                // === BƯỚC 2: "THAM LAM" (VỚ HÀNG LOẠT - .Include()) ===

                // 2.1: Vớ "vé" (BookingFlights)
                .Include(b => b.BookingFlights)
                    // 2.1.1: Từ "vé", vớ "ghế" (Seat) (để lấy SeatNumber)
                    .ThenInclude(bf => bf.Seat)

                // 2.2: Vớ "chuyến bay" (FlightInstance) (vì 'vé' chỉ có ID chuyến bay)
                // (Lưu ý: Ta phải "vớ" từ cái "vé" đầu tiên, vì 1 đơn có thể có 2 chuyến)
                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.FlightInstance)
                        // 2.2.1: Từ "chuyến", vớ "tuyến" (Flight) (để lấy FlightNumber)
                        .ThenInclude(fi => fi.Flight)

                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.FlightInstance)
                        // 2.2.2: Từ "chuyến", vớ "sân bay đi" (để lấy "SGN")
                        .ThenInclude(fi => fi.DepartureAirportNavigation)

                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.FlightInstance)
                        // 2.2.3: Từ "chuyến", vớ "sân bay đến" (để lấy "HAN")
                        .ThenInclude(fi => fi.ArrivalAirportNavigation)

                .AsNoTracking() // (Vì là "chỉ đọc", không "sửa", nên "đọc lẹ")

                // === BƯỚC 3: "NHÀO NẶN" (SELECT) ===
                // "Nặn" cái "đống" Entity "nặng đô" ở trên thành cái DTO "nhẹ bẫng"
                .Select(b => new BookingHistoryDto
                {
                    // Lấy từ "Đơn mẹ" (Booking)
                    BookingId = b.BookingId,
                    BookingCode = b.BookingCode,
                    BookingStatus = b.BookingStatus,
                    TotalAmount = b.TotalAmount,
                    CreatedAt = (DateTime)b.CreatedAt,

                    // Lấy từ "Cháu" (FlightInstance & Flight)
                    // (Ta 'First' vì 1 đơn hàng (booking) chỉ bay 1 chuyến (instance))
                    FlightNumber = b.BookingFlights.First().FlightInstance.Flight.FlightNumber,
                    DepartureAirport = b.BookingFlights.First().FlightInstance.DepartureAirportNavigation.AirportCode,
                    ArrivalAirport = b.BookingFlights.First().FlightInstance.ArrivalAirportNavigation.AirportCode,
                    DepartureTime = b.BookingFlights.First().FlightInstance.DepartureTime,
                    ArrivalTime = b.BookingFlights.First().FlightInstance.ArrivalTime,

                    // "Nặn" list "chắt" (Passengers)
                    Passengers = b.BookingFlights.Select(bf => new HistoryPassengerDto
                    {
                        PassengerName = bf.PassengerName,
                        // Check xem "ghế" có "lót" (Include) được không
                        SeatNumber = bf.Seat != null ? bf.Seat.SeatNumber : null
                    }).ToList()
                })
                .ToListAsync();

            return bookings;
        }

        private async Task<BookingResponseDto> GetBookingResponseDtoById(int bookingId)
        {
            // 1. "Xuống kho"
            var booking = await _context.Bookings
                // 2. "Tham lam": Vớ "vé"
                .Include(b => b.BookingFlights)
                    // 2.1: Từ "vé", vớ "chuyến bay"
                    .ThenInclude(bf => bf.FlightInstance)
                        // 2.1.1: Từ "chuyến", vớ "sân bay đi/đến"
                        .ThenInclude(fi => fi.DepartureAirportNavigation)
                .Include(b => b.BookingFlights)
                    .ThenInclude(bf => bf.FlightInstance)
                        .ThenInclude(fi => fi.ArrivalAirportNavigation)
                // 3. Lọc
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BookingId == bookingId);

            if (booking == null)
            {
                throw new KeyNotFoundException("Lỗi nội bộ: Không tìm thấy đơn hàng sau khi xử lý.");
            }

            // 4. "In" (Map)
            var flight = booking.BookingFlights.First().FlightInstance; // Lấy "chuyến"

            return new BookingResponseDto
            {
                BookingId = booking.BookingId,
                BookingCode = booking.BookingCode,
                TotalAmount = booking.TotalAmount,
                BookingStatus = booking.BookingStatus,
                CreatedAt = (DateTime)booking.CreatedAt,
                FlightInfo = $"{flight.DepartureAirportNavigation.AirportCode} -> {flight.ArrivalAirportNavigation.AirportCode}",
                BookingFlights = booking.BookingFlights.Select(bf => new BookingFlightIdDto
                {
                    BookingFlightId = bf.BookingFlightId,
                    PassengerName = bf.PassengerName
                }).ToList()
            };
        }

        public async Task<BookingResponseDto> AddServiceToBookingAsync(int bookingId, AddServiceRequestDto dto, int accountId)
        {
            // === BƯỚC 1: MỞ "LƯỚI AN TOÀN" (Transaction) ===
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // === BƯỚC 2: KIỂM TRA "HÀNG" ===

                // 2.1: Tìm "Đơn mẹ" (Booking)
                var booking = await _context.Bookings.FindAsync(bookingId);
                if (booking == null) throw new KeyNotFoundException("Đơn hàng (Booking) không tồn tại.");

                // 2.2: Tìm "Dịch vụ" (Service)
                var service = await _context.Services.FindAsync(dto.ServiceId);
                if (service == null) throw new KeyNotFoundException("Dịch vụ (Service) không tồn tại.");

                // === BƯỚC 3: KIỂM TRA "AN NINH" & "LOGIC" ===

                // 3.1: AN NINH (Check "chủ")
                if (booking.AccountId != accountId)
                {
                    throw new Exception("Bạn không có quyền sửa đơn hàng của người khác.");
                }

                // 3.2: LOGIC (Check "trạng thái")
                // Chỉ cho "thêm" khi đơn còn "Pending" (Chờ)
                if (booking.BookingStatus != "Pending")
                {
                    throw new InvalidOperationException($"Không thể thêm. Đơn hàng đang ở trạng thái '{booking.BookingStatus}'.");
                }

                // === BƯỚC 4: TÍNH TOÁN & "GHI SỔ" ===

                // 4.1: Tính tiền cho "hàng" mới
                decimal serviceTotalPrice = (decimal)(service.Price * dto.Quantity);

                // 4.2: Check xem "hàng" này đã "thêm" (Add) bao giờ chưa?
                var existingBookingService = await _context.BookingServices
                    .FirstOrDefaultAsync(bs => bs.BookingId == bookingId && bs.ServiceId == dto.ServiceId);

                if (existingBookingService != null)
                {
                    // A! Có rồi -> "Sửa" (Update) số lượng
                    existingBookingService.Quantity += dto.Quantity;
                    existingBookingService.TotalPrice += serviceTotalPrice;
                }
                else
                {
                    // A! Chưa có -> "Tạo" (Create) "dòng" mới
                    var newBookingService = new BookingService // (Đây là "Tuấn Két Sắt" - Entity)
                    {
                        BookingId = bookingId,
                        ServiceId = dto.ServiceId,
                        Quantity = dto.Quantity,
                        TotalPrice = serviceTotalPrice
                    };
                    _context.BookingServices.Add(newBookingService);
                }

                // 4.3: "Cập nhật" (Update) "Đơn mẹ" (TotalAmount)
                // "Cộng dồn" tiền "hàng" mới vào "tổng"
                booking.TotalAmount += serviceTotalPrice;

                // === BƯỚC 5: LƯU & "CHỐT" TRANSACTION ===
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // === BƯỚC 6: GỌI "MÁY IN" XỊN ===
                // "In" lại cái "biên nhận" (đã "tăng tiền")
                return await GetBookingResponseDtoById(bookingId);
            }
            catch (Exception ex)
            {
                // === BƯỚC X (LỖI!): "QUAY TUA" (Rollback) ===
                await transaction.RollbackAsync();
                throw new Exception($"Thêm dịch vụ thất bại. {ex.Message}");
            }
        }
    }
}