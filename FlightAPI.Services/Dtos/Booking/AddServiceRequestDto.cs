using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Booking
{
    public class AddServiceRequestDto
    {
        [Required]
        public int ServiceId { get; set; } // "Tôi muốn 'Suất ăn' (ID: 1)"

        [Range(1, 100)]
        public int Quantity { get; set; } = 1; // "Số lượng: 1"
    }
}