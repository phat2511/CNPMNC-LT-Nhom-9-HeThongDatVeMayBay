using System.Collections.Generic;
using System.Threading.Tasks;
using FlightAPI.Models; // Sử dụng các DTOs đã được định nghĩa

namespace FlightAPI.Services
{
    // Giao diện (Interface) định nghĩa các hoạt động CRUD cho Flight
    public interface IFlightManagerService
    {
        // Đọc tất cả các chuyến bay cơ sở
        Task<IEnumerable<FlightManagerServiceDto>> GetAllAsync();

        // Đọc chuyến bay cơ sở theo ID
        Task<FlightManagerServiceDto?> GetByIdAsync(int id);

        // Tạo mới chuyến bay cơ sở
        Task<FlightManagerServiceDto> CreateAsync(FlightManagerServiceCreateRequest request);

        // Cập nhật thông tin chuyến bay cơ sở
        Task UpdateAsync(int id, FlightManagerServiceUpdateRequest request);

        // Xóa chuyến bay cơ sở
        Task DeleteAsync(int id);
    }
}
