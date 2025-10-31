// Dùng để hiển thị kết quả tìm kiếm

namespace FlightAPI.Data.Models
{
    public class FlightInstanceReadDto
    {
        public int FlightInstanceId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string AirlineName { get; set; } = string.Empty; // Tên Hãng hàng không

        // Thông tin Sân bay đi
        public string DepartureAirportCode { get; set; } = string.Empty;
        public string DepartureCity { get; set; } = string.Empty;

        // Thông tin Sân bay đến
        public string ArrivalAirportCode { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;

        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal BasePrice { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}