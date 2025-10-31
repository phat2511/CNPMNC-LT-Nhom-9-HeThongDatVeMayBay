using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Booking
{
    public class PassengerDto
    {
        [Required]
        public string PassengerName { get; set; } = null!;

        [Required]
        // Kiểu hành khách: ADT (Người lớn), CHD (Trẻ em), INF (Em bé)
        public string PassengerType { get; set; } = "ADT";
    }
}