using FlightAPI.Data.Models;

namespace FlightAPI.Services
{
    public interface IFlightInstanceService
    {
        // Hàm tìm kiếm cốt lõi
        Task<IEnumerable<FlightInstanceReadDto>> SearchFlightsAsync(string depCode, string arrCode, DateTime date);

        // Hàm CRUD cơ bản
        Task<FlightInstanceReadDto?> GetInstanceByIdAsync(int id);
        Task<FlightInstanceReadDto> CreateInstanceAsync(FlightInstanceCreateDto dto);
        // ... (Thêm Update/Delete)4
        Task<bool> UpdateInstanceAsync(int id, FlightInstanceCreateDto dto);
        Task<bool> DeleteInstanceAsync(int id);
    }
}