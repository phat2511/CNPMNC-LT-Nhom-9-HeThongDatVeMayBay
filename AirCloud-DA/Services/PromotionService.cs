using AirCloud_DA.Data;
using Microsoft.EntityFrameworkCore;

namespace AirCloud_DA.Services
{
    public class PromotionService : IPromotionService
    {
        private readonly AirCloudDbContext _context;

        public PromotionService(AirCloudDbContext context)
        {
            _context = context;
        }

        public async Task<Promotion> GetPromotionByIdAsync(int promotionId)
        {
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.PromotionId == promotionId);
        }

        public async Task<List<Promotion>> GetActivePromotionsAsync()
        {
            return await _context.Promotions
                .Where(p => p.IsActive == true &&
                           (p.StartDate == null || p.StartDate <= DateOnly.FromDateTime(DateTime.Now)) &&
                           (p.EndDate == null || p.EndDate >= DateOnly.FromDateTime(DateTime.Now)))
                .OrderBy(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<List<Promotion>> GetAllPromotionsAsync()
        {
            return await _context.Promotions
                .OrderBy(p => p.PromotionId)
                .ToListAsync();
        }

        public async Task<List<Promotion>> GetPromotionsFilteredAsync(bool? isActive, DateOnly? from, DateOnly? to, bool oldestFirst)
        {
            var query = _context.Promotions.AsQueryable();

            if (isActive.HasValue)
                query = query.Where(p => p.IsActive == isActive);

            if (from.HasValue)
                query = query.Where(p => !p.StartDate.HasValue || p.StartDate >= from);

            if (to.HasValue)
                query = query.Where(p => !p.EndDate.HasValue || p.EndDate <= to);

            query = oldestFirst ? query.OrderBy(p => p.PromotionId)
                                : query.OrderByDescending(p => p.PromotionId);

            return await query.ToListAsync();
        }

        public async Task<Promotion?> GetPromotionByCodeAsync(string code)
        {
            return await _context.Promotions
                .FirstOrDefaultAsync(p => p.Code == code);
        }

        public async Task<Promotion?> ValidatePromotionAsync(string code, decimal orderAmount)
        {
            var promotion = await GetPromotionByCodeAsync(code);

            if (promotion == null || promotion.IsActive != true)
                return null;

            var now = DateOnly.FromDateTime(DateTime.Now);
            if (promotion.StartDate.HasValue && promotion.StartDate > now)
                return null;
            if (promotion.EndDate.HasValue && promotion.EndDate < now)
                return null;

            return promotion;
        }

        public async Task CreatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task UpdatePromotionAsync(Promotion promotion)
        {
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();
        }

        public async Task DeletePromotionAsync(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion != null)
            {
                _context.Promotions.Remove(promotion);
                await _context.SaveChangesAsync();
            }
        }
    }
}