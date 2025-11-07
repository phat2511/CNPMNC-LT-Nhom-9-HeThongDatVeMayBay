// Thêm 3 'using' quan trọng này
using FlightAPI.Services;
using FlightAPI.Services.Dtos.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FlightAPI.Controllers // Đảm bảo namespace này khớp với các Controller khác
{
    [Route("api/[controller]")] // <-- "Bùa chú" #1: Đường dẫn
    [ApiController]             // <-- "Bùa chú" #2: Báo đây là API Controller
    public class AuthController : ControllerBase // <-- "Bùa chú" #3: Kế thừa
    {
        private readonly IAuthService _authService;

        // DI "tiêm" IAuthService vào
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequestDto dto)
        {
            try
            {
                await _authService.VerifyEmailAsync(dto);
                return Ok(new { message = "Xác thực email thành công. Bạn có thể đăng nhập." });
            }
            catch (Exception ex)
            {
                // (Bắt lỗi "Mã sai" hoặc "Hết hạn")
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                await _authService.RegisterAsync(dto); // <--- "Não" mới sẽ "Không" trả gì
                return Ok(new { message = "Đăng ký thành công. Vui lòng check email (hoặc console) để kích hoạt tài khoản." });
            }
            catch (Exception ex)
            {
                // Bắt các lỗi như "Username đã tồn tại"
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto dto)
        {
            try
            {
                var result = await _authService.LoginAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                // Bắt các lỗi như "Sai Username hoặc Password"
                return BadRequest(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUserProfile()
        {
            // Token đã được xác thực, ta có thể tin tưởng các Claim bên trong nó
            var username = User.Identity?.Name;
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Lấy ID
            var fullName = User.FindFirstValue("fullname"); // Lấy claim custom

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            // Trả về thông tin lấy TỪ TOKEN
            return Ok(new
            {
                Id = userId,
                FullName = fullName,
                UserName = username,
                Role = User.FindFirstValue(ClaimTypes.Role) // Lấy role
            });
        }
    }
}