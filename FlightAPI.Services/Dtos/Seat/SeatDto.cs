namespace FlightAPI.Services.Dtos.Seat // (Hoặc namespace của sếp)
{
    public class SeatDto
    {
        public int SeatId { get; set; }
        public string SeatNumber { get; set; } = null!;
        public string SeatClassName { get; set; } = null!;
        public decimal PriceMultiplier { get; set; }
        public bool IsAvailable { get; set; }
    }
}