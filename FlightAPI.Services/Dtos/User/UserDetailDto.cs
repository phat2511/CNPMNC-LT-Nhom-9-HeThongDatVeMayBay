namespace FlightAPI.Services.Dtos.User
{
    public class UserDetailDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }

        // "Hàng xịn" ta "móc" (JOIN) qua
        public string RoleName { get; set; } = null!;

        // "Tình trạng"
        public bool IsLocked { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}