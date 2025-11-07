using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Booking
{
    public class BookingRequestDto
    {
        [Required]
        public int FlightInstanceId { get; set; }

        // Mặc định là hạng Economy (Phổ thông)
        public int SeatClassId { get; set; } = 1;

        [Required]
        [MinLength(1)] // Phải có ít nhất 1 hành khách
        public List<PassengerDto> Passengers { get; set; } = new List<PassengerDto>();

        public string? PromotionCode { get; set; }
    }
}