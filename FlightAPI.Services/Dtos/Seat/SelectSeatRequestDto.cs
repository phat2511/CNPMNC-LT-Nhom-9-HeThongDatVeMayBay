using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Services.Dtos.Booking // (Để chung với Booking DTOs cũng được)
{
    public class SelectSeatRequestDto
    {
        [Required]
        // Đây là ID của vé (hành khách) cụ thể
        public int BookingFlightId { get; set; }

        [Required]
        // Đây là ID của cái ghế họ muốn chọn
        public int SeatId { get; set; }
    }
}