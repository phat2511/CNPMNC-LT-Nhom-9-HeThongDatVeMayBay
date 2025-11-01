namespace FlightAPI.Services.Dtos.Service
{
    public class ServiceDto
    {
        public int ServiceId { get; set; }
        public string Name { get; set; } = null!;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
    }
}