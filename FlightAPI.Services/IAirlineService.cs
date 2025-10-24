using System.Collections.Generic;
using System.Threading.Tasks;
using FlightAPI.Models;

namespace FlightAPI.Services
{
    public interface IAirlineService
    {
        Task<IEnumerable<AirlineCreateDto>> GetAllAsync();
        Task<AirlineCreateDto?> GetByCodeAsync(string code); // Sử dụng Code làm PK
        Task<AirlineCreateDto> CreateAsync(AirlineCreateDto request);
        Task UpdateAsync(string code, AirlineCreateDto request);
        Task DeleteAsync(string code);
    }
}
