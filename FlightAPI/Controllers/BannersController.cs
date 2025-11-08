using FlightAPI.Services; // Thêm
using FlightAPI.Services.Dtos.Banner; // Thêm
using Microsoft.AspNetCore.Authorization; // Thêm
using Microsoft.AspNetCore.Mvc;

namespace FlightAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // "Vệ sĩ" Admin
    public class BannersController : ControllerBase
    {
        private readonly IBannerService _bannerService;
        

        public BannersController(IBannerService bannerService)
        {
            _bannerService = bannerService;
        }

        // 1. GET (Read All)
        [HttpGet]
        public async Task<IActionResult> GetAllBanners()
        {
            var banners = await _bannerService.GetAllAsync();
            return Ok(banners);
        }

        // 2. GET (Read One)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBannerById(int id)
        {
            var banner = await _bannerService.GetByIdAsync(id);
            if (banner == null) return NotFound("Không tìm thấy banner.");
            return Ok(banner);
        }

        // 3. POST (Create)
        // (Trong BannersController.cs)
        [HttpPost]
        // SỬA LỖI: Dùng [FromForm] để nhận file và dữ liệu form
        public async Task<IActionResult> CreateBanner([FromForm] BannerRequestDto dto)
        {
            // Kiểm tra tính hợp lệ của Model trước khi xử lý
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra file (giữ nguyên logic của bạn)
            if (dto.File == null || dto.File.Length == 0)
            {
                return BadRequest(new { message = "Vui lòng chọn file ảnh hợp lệ." });
            }

            try
            {
                var newBanner = await _bannerService.CreateAsync(dto);
                // Trả về 201 Created và thông tin Banner
                return CreatedAtAction(nameof(GetBannerById), new { id = newBanner.BannerId }, newBanner);
            }
            catch (Exception ex)
            {
                // Xử lý lỗi trong quá trình Service (ví dụ: lỗi upload Cloudinary)
                return StatusCode(500, new { message = "Lỗi tạo banner: " + ex.Message });
            }
        }

        // 4. PUT (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBanner(int id, [FromBody] BannerRequestDto dto)
        {
            try
            {
                if (dto.File != null && dto.File.Length == 0)
                {
                    return BadRequest(new { message = "File ảnh không hợp lệ." });
                }

                var updatedBanner = await _bannerService.UpdateAsync(id, dto);
                return Ok(updatedBanner);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 5. DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBanner(int id)
        {
            try
            {
                await _bannerService.DeleteAsync(id);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}