// Dùng cho Admin khi tạo lịch trình chuyến bay

namespace FlightAPI.Data.Models
{
    public class FlightInstanceCreateDto
    {
        // Khóa ngoại đến bảng core.Flight
        public int FlightId { get; set; }

        public string DepartureAirport { get; set; } = string.Empty;
        public string ArrivalAirport { get; set; } = string.Empty;

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal BasePrice { get; set; }
    }
}