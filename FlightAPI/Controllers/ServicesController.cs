using FlightAPI.Services; // Thêm
using FlightAPI.Services.Dtos.Service; // Thêm
using Microsoft.AspNetCore.Authorization; // Thêm
using Microsoft.AspNetCore.Mvc;

namespace FlightAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // === VỆ SĨ VIP ===
    // Chỉ "thằng" nào có token, VÀ token đó có Role là "Admin" 
    // (từ bảng auth.Role của sếp) thì MỚI được vào "cái quầy" này.
    [Authorize(Roles = "Admin")]
    public class ServicesController : ControllerBase
    {
        // (Tên 'IServiceService' nó hơi "ngu", nhưng ta cứ theo chuẩn)
        private readonly IServiceService _serviceService;

        public ServicesController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        // 1. GET (Read All)
        [HttpGet]
        public async Task<IActionResult> GetAllServices()
        {
            var services = await _serviceService.GetAllAsync();
            return Ok(services);
        }

        // 2. GET (Read One)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetServiceById(int id)
        {
            var service = await _serviceService.GetByIdAsync(id);
            if (service == null) return NotFound("Không tìm thấy dịch vụ.");
            return Ok(service);
        }

        // 3. POST (Create)
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] ServiceRequestDto dto)
        {
            var newService = await _serviceService.CreateAsync(dto);
            // Trả về 201 Created, kèm link (chuẩn REST) và "hàng" vừa tạo
            return CreatedAtAction(nameof(GetServiceById), new { id = newService.ServiceId }, newService);
        }

        // 4. PUT (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceRequestDto dto)
        {
            try
            {
                var updatedService = await _serviceService.UpdateAsync(id, dto);
                return Ok(updatedService);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 5. DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(int id)
        {
            try
            {
                await _serviceService.DeleteAsync(id);
                return NoContent(); // 204 No Content (Xóa thành công, không trả gì)
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}