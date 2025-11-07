using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.User
{
    public class UserLockRequestDto
    {
        [Required]
        public bool Lock { get; set; } // true = Khóa, false = Mở

        public string? LockReason { get; set; }
    }
}