using FlightAPI.Services.Dtos.Promotion;

namespace FlightAPI.Services
{
    public interface IPromotionService
    {
        Task<IEnumerable<PromotionReadDto>> GetAllAsync();
        Task<PromotionReadDto?> GetByIdAsync(int id);
        Task<PromotionReadDto> CreateAsync(PromotionRequestDto dto);
        Task<PromotionReadDto> UpdateAsync(int id, PromotionRequestDto dto);
        Task DeleteAsync(int id);
    }
}