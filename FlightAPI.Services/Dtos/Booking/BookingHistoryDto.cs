namespace FlightAPI.Services.Dtos.Booking
{
    public class BookingHistoryDto
    {
        // Thông tin Đơn hàng
        public int BookingId { get; set; }
        public string BookingCode { get; set; } = null!;
        public string BookingStatus { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime CreatedAt { get; set; }

        // Thông tin Chuyến bay
        public string FlightNumber { get; set; } = null!;
        public string DepartureAirport { get; set; } = null!; // "SGN"
        public string ArrivalAirport { get; set; } = null!; // "HAN"
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }

        // "Móc" tờ "biên lai con" vào
        public List<HistoryPassengerDto> Passengers { get; set; } = new List<HistoryPassengerDto>();
    }
}