using FlightAPI.Services.Dtos.Service;

namespace FlightAPI.Services
{
    public interface IServiceService
    {
        Task<IEnumerable<ServiceDto>> GetAllAsync();
        Task<ServiceDto?> GetByIdAsync(int id);
        Task<ServiceDto> CreateAsync(ServiceRequestDto dto);
        Task<ServiceDto> UpdateAsync(int id, ServiceRequestDto dto);
        Task DeleteAsync(int id);
    }
}