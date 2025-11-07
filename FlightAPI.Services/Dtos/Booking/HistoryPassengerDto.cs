namespace FlightAPI.Services.Dtos.Booking
{
    public class HistoryPassengerDto
    {
        public string PassengerName { get; set; } = null!;
        public string? SeatNumber { get; set; } // (Có thể null nếu chưa chọn)
    }
}