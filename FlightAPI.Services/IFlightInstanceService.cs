using FlightAPI.Data.Models; // Giả định các DTO khác cũng ở đây
using FlightAPI.Services.Dtos.Seat;

namespace FlightAPI.Services
{
    public interface IFlightInstanceService
    {
        // ... (Các phương thức GetAll, GetById, Create, Update, Delete) ...
        Task<IEnumerable<FlightInstanceReadDto>> GetAllAsync();
        Task<FlightInstanceReadDto?> GetInstanceByIdAsync(int id);
        Task<FlightInstanceReadDto> CreateInstanceAsync(FlightInstanceCreateDto dto);
        Task<bool> UpdateInstanceAsync(int id, FlightInstanceCreateDto dto);
        Task<bool> DeleteInstanceAsync(int id);

        Task<IEnumerable<SeatDto>> GetSeatsForFlightAsync(int flightInstanceId);

        // =======================================================
        // ĐÃ SỬA: Thêm '?' để cho phép tham số tùy chọn (nullable)
        // =======================================================
        Task<IEnumerable<FlightInstanceReadDto>> SearchFlightsAsync(
            string? depCode, string? arrCode, DateTime? date);
    }
}

