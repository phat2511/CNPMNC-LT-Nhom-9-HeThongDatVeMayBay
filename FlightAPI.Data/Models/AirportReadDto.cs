// FlightAPI.Data/Models/AirportReadDto.cs

namespace FlightAPI.Data.Models
{
    public class AirportReadDto
    {
        public string AirportCode { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}