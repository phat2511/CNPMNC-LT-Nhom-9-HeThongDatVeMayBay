using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Models
{
    // Thường giống với Create DTO, đảm bảo các ràng buộc dữ liệu được duy trì.
    public class FlightManagerServiceUpdateRequest
    {
        [Required(ErrorMessage = "Số hiệu chuyến bay là bắt buộc.")]
        [StringLength(10, ErrorMessage = "Số hiệu chuyến bay không được quá 10 ký tự.")]
        public string FlightNumber { get; set; }

        public string AirlineCode { get; set; }
    }
}
