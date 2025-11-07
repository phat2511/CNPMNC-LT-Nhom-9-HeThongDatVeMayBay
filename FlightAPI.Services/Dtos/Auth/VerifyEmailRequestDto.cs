using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Auth
{
    public class VerifyEmailRequestDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string Code { get; set; } = null!;
    }
}