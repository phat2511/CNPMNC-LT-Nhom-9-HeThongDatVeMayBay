namespace FlightAPI.Models
{
    public class FlightManagerServiceDto
    {
        // Thêm Id vì đây là dữ liệu trả về đã được tạo
        public int Id { get; set; }
        public string FlightNumber { get; set; }

        public string AirlineCode { get; set; }
    }
}
