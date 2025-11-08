using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace FlightAPI.Services.Dtos.Banner
{
    public class BannerRequestDto
    {
        [Required]
        [StringLength(150)]
        public string Title { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng chọn file ảnh.")]
        public IFormFile File { get; set; } = null!;

        [StringLength(255)]
        public string? LinkUrl { get; set; } // "/search?code=SALE10"

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        // "Móc" cái banner này vào cái "Deal" sếp tạo lúc nãy
        public int? PromotionId { get; set; }
    }
}