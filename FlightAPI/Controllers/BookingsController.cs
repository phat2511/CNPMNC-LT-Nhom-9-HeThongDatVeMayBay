using FlightAPI.Data.Entities;
using FlightAPI.Services; // Thêm
using FlightAPI.Services.Dtos.Booking; // Thêm
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

        // ... (Chúng ta sẽ thêm API chọn ghế, thêm dịch vụ vào đây sau) ...
    }
}