using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Models
{
    // DTO để tạo mới (POST)
    public class AirlineCreateDto
    {
        [Required]
        [StringLength(3)]
        public string AirlineCode { get; set; }  // Ví dụ: VN, VJ

        [Required]
        [StringLength(100)]
        public string Name { get; set; } // Ví dụ: Vietnam Airlines

        [Required]
        [StringLength(100)]
        public string Website { get; set; }
    }

    // DTO để cập nhật (PUT) - Code thường không thay đổi, chỉ thay đổi Name
    public class AirlineUpdateRequest
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

    }

    // DTO để hiển thị (GET)
    public class AirlineDto
    {
        public string AirlineCode { get; set; }
        public string Name { get; set; }

        [Required]
        [StringLength(100)]
        public string Website { get; set; }
    }
}
