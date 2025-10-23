using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Models
{
    public class FlightManagerServiceCreateRequest
    {
        [Required(ErrorMessage = "Số hiệu chuyến bay là bắt buộc.")]
        [StringLength(10, ErrorMessage = "Số hiệu chuyến bay không được quá 10 ký tự.")]
        public string FlightNumber { get; set; }
    }
}
