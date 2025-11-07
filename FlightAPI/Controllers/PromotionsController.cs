using FlightAPI.Services; // Thêm
using FlightAPI.Services.Dtos.Promotion; // Thêm
using Microsoft.AspNetCore.Authorization; // Thêm
using Microsoft.AspNetCore.Mvc;

namespace FlightAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")] // "Vệ sĩ" Admin
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        // 1. GET (Read All)
        [HttpGet]
        public async Task<IActionResult> GetAllPromotions()
        {
            var promotions = await _promotionService.GetAllAsync();
            return Ok(promotions);
        }

        // 2. GET (Read One)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetPromotionById(int id)
        {
            var promotion = await _promotionService.GetByIdAsync(id);
            if (promotion == null) return NotFound("Không tìm thấy khuyến mãi.");
            return Ok(promotion);
        }

        // 3. POST (Create)
        [HttpPost]
        public async Task<IActionResult> CreatePromotion([FromBody] PromotionRequestDto dto)
        {
            // (Nếu DTO "Validate" (ở Bước 1) thất bại, nó "chết" ở đây, 400 Bad Request)
            try
            {
                var newPromotion = await _promotionService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetPromotionById), new { id = newPromotion.PromotionId }, newPromotion);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        // 4. PUT (Update)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePromotion(int id, [FromBody] PromotionRequestDto dto)
        {
            try
            {
                var updatedPromotion = await _promotionService.UpdateAsync(id, dto);
                return Ok(updatedPromotion);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        // 5. DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePromotion(int id)
        {
            try
            {
                await _promotionService.DeleteAsync(id);
                return NoContent(); // 204
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}