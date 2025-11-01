using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Service
{
    public class ServiceRequestDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [Range(0, 99999999)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
    }
}