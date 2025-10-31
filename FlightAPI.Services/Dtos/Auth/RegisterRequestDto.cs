using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Auth // <-- Namespace đúng
{
    public class RegisterRequestDto // <-- Class là PUBLIC
    {
        [Required] public string Username { get; set; }
        [Required][EmailAddress] public string Email { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string Password { get; set; }
    }
}