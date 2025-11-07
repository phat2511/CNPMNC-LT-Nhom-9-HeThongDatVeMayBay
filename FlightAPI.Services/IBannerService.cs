using FlightAPI.Services.Dtos.Banner;

namespace FlightAPI.Services
{
    public interface IBannerService
    {
        // Trả về "hàng xịn" (DTO có tên "Deal")
        Task<IEnumerable<BannerReadDto>> GetAllAsync();
        Task<BannerReadDto?> GetByIdAsync(int id);
        Task<BannerReadDto> CreateAsync(BannerRequestDto dto);
        Task<BannerReadDto> UpdateAsync(int id, BannerRequestDto dto);
        Task DeleteAsync(int id);
    }
}