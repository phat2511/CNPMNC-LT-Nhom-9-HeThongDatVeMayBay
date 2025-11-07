using FlightAPI.Data.Entities;
using FlightAPI.Services; // Thêm
using FlightAPI.Services.Dtos.Booking; // Thêm
using FlightAPI.Services.Dtos.Payment;
using Microsoft.AspNetCore.Authorization; // Thêm
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims; // Thêm

namespace FlightAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // <--- CỰC KỲ QUAN TRỌNG: Chỉ ai có token mới được đặt vé
    public class BookingsController : ControllerBase
    {
        private readonly IBookedService _bookingService;

        public BookingsController(IBookedService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingRequestDto dto)
        {
            try
            {
                // Lấy AccountId (dạng string) từ token
                var accountIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(accountIdString))
                {
                    return Unauthorized("Token không hợp lệ hoặc không có AccountId.");
                }

                // Chuyển AccountId sang int
                if (!int.TryParse(accountIdString, out int accountId))
                {
                    return Unauthorized("AccountId trong token không phải là số.");
                }

                // Gọi Service để xử lý
                var result = await _bookingService.CreateBookingAsync(dto, accountId);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // Lỗi 404 (Không tìm thấy chuyến bay)
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // Lỗi 400 (Hết vé, v.v.)
            }
        }

        [HttpPut("select-seat")] // <--- API "CHỌN"
        public async Task<IActionResult> SelectSeat([FromBody] SelectSeatRequestDto dto)
        {
            try
            {
                var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _bookingService.SelectSeatAsync(dto, accountId);

                return Ok(new { message = "Chọn ghế thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // ... (Chúng ta sẽ thêm API chọn ghế, thêm dịch vụ vào đây sau) ...
        [HttpPost("{id}/payment")] // <--- API THANH TOÁN
                                   // (Nó nhận 'id' là 'BookingId' từ đường dẫn)
        public async Task<IActionResult> ProcessPayment([FromRoute] int id, [FromBody] PaymentRequestDto dto)
        {
            try
            {
                var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Gọi "thợ" vào xử lý (ta sẽ làm "não" ở bước sau)
                var result = await _bookingService.ProcessPaymentAsync(id, dto, accountId);

                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404 (Không tìm thấy đơn)
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 (Đơn đã trả, đơn hết hạn)
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("my-history")] // <--- API LỊCH SỬ VÉ
        [Authorize] // (Bất kỳ ai 'login' đều xem được, không cần 'Admin')
        public async Task<IActionResult> GetMyHistory()
        {
            try
            {
                var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Gọi "thợ" vào xử lý (ta sẽ làm "não" ở bước sau)
                var history = await _bookingService.GetMyBookingHistoryAsync(accountId);

                return Ok(history);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{bookingId}/services")] // <--- API THÊM DỊCH VỤ
        [Authorize] // (Tất nhiên là phải "login")
        public async Task<IActionResult> AddServiceToBooking([FromRoute] int bookingId, [FromBody] AddServiceRequestDto dto)
        {
            try
            {
                var accountId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                // Gọi "thợ" vào xử lý (ta sẽ làm "não" ở bước sau)
                // Nó sẽ "nhả" lại cái đơn hàng đã "tăng tiền"
                var updatedBooking = await _bookingService.AddServiceToBookingAsync(bookingId, dto, accountId);

                return Ok(updatedBooking); // Trả về "biên nhận" mới
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message }); // 404 (Không tìm thấy đơn/dịch vụ)
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message }); // 400 (Dịch vụ đã có, đơn đã hủy...)
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message }); // <--- SỬA THÀNH DẤU "="
            }
        }
    }
}