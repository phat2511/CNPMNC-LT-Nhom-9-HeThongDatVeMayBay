using AirCloud_DA.Data;
using Microsoft.EntityFrameworkCore;

namespace AirCloud_DA.Services
{
    public class BannerService : IBannerService
    {
        private readonly AirCloudDbContext _context;

        public BannerService(AirCloudDbContext context)
        {
            _context = context;
        }

        public async Task<List<Banner>> GetActiveBannersAsync()
        {
            return await _context.Banners
                .Where(b => b.IsActive == true &&
                           (b.StartDate == null || b.StartDate <= DateTime.Now) &&
                           (b.EndDate == null || b.EndDate >= DateTime.Now))
                .OrderBy(b => b.StartDate)
                .ToListAsync();
        }

        public async Task<List<Banner>> GetAllBannersAsync()
        {
            return await _context.Banners
                .Include(b => b.CreatedByNavigation)
                .Include(b => b.Promotion)
                .OrderBy(b => b.BannerId)
                .ToListAsync();
        }

        public async Task<Banner?> GetBannerByIdAsync(int id)
        {
            return await _context.Banners
                .Include(b => b.CreatedByNavigation)
                .Include(b => b.Promotion)
                .FirstOrDefaultAsync(b => b.BannerId == id);
        }

        public async Task CreateBannerAsync(Banner banner)
        {
            banner.CreatedAt = DateTime.Now;
            _context.Banners.Add(banner);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateBannerAsync(Banner banner)
        {
            _context.Banners.Update(banner);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBannerAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Banner>> GetBannersFilteredAsync(bool? isActive, DateTime? from, DateTime? to, bool oldestFirst)
        {
            var query = _context.Banners
                .Include(b => b.CreatedByNavigation)
                .Include(b => b.Promotion)
                .AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(b => b.IsActive == isActive);
            }

            if (from.HasValue)
            {
                var f = from.Value;
                query = query.Where(b => !b.StartDate.HasValue || b.StartDate >= f);
            }

            if (to.HasValue)
            {
                var t = to.Value;
                query = query.Where(b => !b.EndDate.HasValue || b.EndDate <= t);
            }

            query = oldestFirst ? query.OrderBy(b => b.BannerId) : query.OrderByDescending(b => b.BannerId);

            return await query.ToListAsync();
        }
    }
}