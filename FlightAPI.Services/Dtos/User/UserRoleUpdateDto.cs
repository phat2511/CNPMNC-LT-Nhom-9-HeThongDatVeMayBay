using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.User
{
    public class UserRoleUpdateDto
    {
        [Required]
        public string NewRoleName { get; set; } = null!; // "Admin", "User", "Staff"...
    }
}