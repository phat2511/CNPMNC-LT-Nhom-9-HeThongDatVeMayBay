using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FlightAPI.Models; // DTOs
using FlightAPI.Data.Models; // Giả định các DTO khác cũng ở đây

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

        // =======================================================
        // ĐÃ SỬA: Thêm '?' để cho phép tham số tùy chọn (nullable)
        // =======================================================
        Task<IEnumerable<FlightInstanceReadDto>> SearchFlightsAsync(
            string? depCode, string? arrCode, DateTime? date);
    }
}

