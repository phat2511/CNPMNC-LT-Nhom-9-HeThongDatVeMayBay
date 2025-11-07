using FlightAPI.Services; // Thêm
using FlightAPI.Services.Dtos.User; // Thêm
using Microsoft.AspNetCore.Authorization; // Thêm
using Microsoft.AspNetCore.Mvc;

namespace FlightAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // "Vệ sĩ" Admin CỨNG
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        // 1. GET (Read All Users)
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userService.GetAllAsync();
            return Ok(users);
        }

        // 2. PUT (Lock/Unlock)
        [HttpPut("{id}/lock")]
        public async Task<IActionResult> ToggleLock(int id, [FromBody] UserLockRequestDto dto)
        {
            try
            {
                await _userService.ToggleLockAsync(id, dto);
                return Ok(new { message = $"Đã cập nhật trạng thái khóa cho User ID: {id}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 3. PUT (Change Role)
        [HttpPut("{id}/role")]
        public async Task<IActionResult> ChangeRole(int id, [FromBody] UserRoleUpdateDto dto)
        {
            try
            {
                await _userService.ChangeRoleAsync(id, dto);
                return Ok(new { message = $"Đã đổi Role cho User ID: {id} thành {dto.NewRoleName}" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}