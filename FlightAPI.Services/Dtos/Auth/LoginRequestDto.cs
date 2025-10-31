using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Auth // <-- Namespace đúng
{
    public class LoginRequestDto // <-- Class là PUBLIC
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
    }
}