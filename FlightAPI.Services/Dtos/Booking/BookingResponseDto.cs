namespace FlightAPI.Services.Dtos.Booking
{
    public class BookingResponseDto
    {
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public string BookingStatus { get; set; } = "Pending";
        public DateTime CreatedAt { get; set; }
        public string FlightInfo { get; set; } = null!; // Mô tả (VD: "SGN -> HAN")

        public List<BookingFlightIdDto> BookingFlights { get; set; } = new List<BookingFlightIdDto>();
    }
}