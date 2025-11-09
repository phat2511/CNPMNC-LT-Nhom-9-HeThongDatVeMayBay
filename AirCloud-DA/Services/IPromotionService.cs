using AirCloud_DA.Data;

namespace AirCloud_DA.Services
{
    public interface IPromotionService
    {
        Task<List<Promotion>> GetActivePromotionsAsync();
        Task<List<Promotion>> GetAllPromotionsAsync();
        Task<Promotion> GetPromotionByIdAsync(int promotionId);
        Task<Promotion?> GetPromotionByCodeAsync(string code);
        Task<Promotion?> ValidatePromotionAsync(string code, decimal orderAmount);
        Task CreatePromotionAsync(Promotion promotion);
        Task UpdatePromotionAsync(Promotion promotion);
        Task DeletePromotionAsync(int id);
        Task<List<Promotion>> GetPromotionsFilteredAsync(bool? isActive, DateOnly? from, DateOnly? to, bool oldestFirst);
    }
}