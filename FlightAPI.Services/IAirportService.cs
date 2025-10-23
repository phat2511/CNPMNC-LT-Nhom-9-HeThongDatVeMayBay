// FlightAPI.Services/IAirportService.cs

using FlightAPI.Data.Models;

namespace FlightAPI.Services
{
    public interface IAirportService
    {
        Task<IEnumerable<AirportReadDto>> GetAllAsync();
        Task<AirportReadDto?> GetByCodeAsync(string code);
        Task<AirportReadDto> CreateAsync(AirportCreateDto dto);
        Task<bool> UpdateAsync(string code, AirportCreateDto dto);
        Task<bool> DeleteAsync(string code);
    }
}