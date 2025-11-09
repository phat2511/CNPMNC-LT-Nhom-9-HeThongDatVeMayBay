using AirCloud_DA.Data;
using Microsoft.EntityFrameworkCore;

namespace AirCloud_DA.Services
{
    public class FlightSearchService : IFlightSearchService
    {
        private readonly AirCloudDbContext _context;

        public FlightSearchService(AirCloudDbContext context)
        {
            _context = context;
        }

        public async Task<List<FlightSearchResult>> SearchFlightsAsync(string from, string to, DateTime date, int passengers, string cabinClass)
        {
            var flights = await _context.FlightInstances
                .Include(fi => fi.Flight)
                .ThenInclude(f => f.AirlineCodeNavigation)
                .Include(fi => fi.DepartureAirportNavigation)
                .Include(fi => fi.ArrivalAirportNavigation)
                .Where(fi => fi.DepartureAirport == from.ToUpper() &&
                           fi.ArrivalAirport == to.ToUpper() &&
                           fi.DepartureTime.Date == date.Date &&
                           fi.Status == "Scheduled")
                .OrderBy(fi => fi.DepartureTime)
                .ToListAsync();

            return flights.Select(fi => new FlightSearchResult
            {
                FlightInstanceId = fi.FlightInstanceId,
                FlightNumber = fi.Flight.FlightNumber,
                Airline = fi.Flight.AirlineCodeNavigation.Name,
                FromAirport = $"{fi.DepartureAirport} - {fi.DepartureAirportNavigation.Name}",
                ToAirport = $"{fi.ArrivalAirport} - {fi.ArrivalAirportNavigation.Name}",
                DepartureTime = fi.DepartureTime,
                ArrivalTime = fi.ArrivalTime,
                Price = CalculatePrice(fi.BasePrice, passengers, cabinClass),
                Status = fi.Status,
                Duration = FormatDuration(fi.ArrivalTime - fi.DepartureTime)
            }).ToList();
        }

        public async Task<List<FlightSearchResult>> SearchFlightsAsync(string from, string to, DateTime departDate, DateTime? returnDate, int passengers, string cabinClass)
        {
            var departFlights = await SearchFlightsAsync(from, to, departDate, passengers, cabinClass);

            if (returnDate.HasValue)
            {
                var returnFlights = await SearchFlightsAsync(to, from, returnDate.Value, passengers, cabinClass);
                return departFlights.Concat(returnFlights).ToList();
            }

            return departFlights;
        }

        public async Task<List<Airport>> GetAirportsAsync()
        {
            return await _context.Airports
                .OrderBy(a => a.City)
                .ThenBy(a => a.Name)
                .ToListAsync();
        }

        private decimal CalculatePrice(decimal basePrice, int passengers, string cabinClass)
        {
            var multiplier = cabinClass switch
            {
                "Economy" => 1.0m,
                "PremiumEconomy" => 1.25m,
                "Business" => 1.8m,
                "First" => 2.5m,
                _ => 1.0m
            };

            return basePrice * multiplier * passengers;
        }

        private string FormatDuration(TimeSpan duration)
        {
            return $"{duration.Hours}h {duration.Minutes}m";
        }
    }
}