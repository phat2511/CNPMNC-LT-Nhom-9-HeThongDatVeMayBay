// FlightAPI.Data/Models/AirportCreateDto.cs

using System.ComponentModel.DataAnnotations;

namespace FlightAPI.Data.Models
{
    public class AirportCreateDto
    {
        [Required]
        [StringLength(5)]
        public string AirportCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}