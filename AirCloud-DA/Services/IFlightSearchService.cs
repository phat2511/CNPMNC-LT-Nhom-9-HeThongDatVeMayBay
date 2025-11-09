using AirCloud_DA.Data;

namespace AirCloud_DA.Services
{
    public interface IFlightSearchService
    {
        Task<List<FlightSearchResult>> SearchFlightsAsync(string from, string to, DateTime date, int passengers, string cabinClass);
        Task<List<FlightSearchResult>> SearchFlightsAsync(string from, string to, DateTime departDate, DateTime? returnDate, int passengers, string cabinClass);
        Task<List<Airport>> GetAirportsAsync();
    }

    public class FlightSearchResult
    {
        public int FlightInstanceId { get; set; }
        public string FlightNumber { get; set; } = string.Empty;
        public string Airline { get; set; } = string.Empty;
        public string FromAirport { get; set; } = string.Empty;
        public string ToAirport { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
    }
}