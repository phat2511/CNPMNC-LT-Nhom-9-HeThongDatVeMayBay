using AirCloud_DA.Data;

namespace AirCloud_DA.Services
{
    public interface IBannerService
    {
        Task<List<Banner>> GetActiveBannersAsync();
        Task<List<Banner>> GetAllBannersAsync();
        Task<Banner?> GetBannerByIdAsync(int id);
        Task CreateBannerAsync(Banner banner);
        Task UpdateBannerAsync(Banner banner);
        Task DeleteBannerAsync(int id);
        Task<List<Banner>> GetBannersFilteredAsync(bool? isActive, DateTime? from, DateTime? to, bool oldestFirst);
    }
}