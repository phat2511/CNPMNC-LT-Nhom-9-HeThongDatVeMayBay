using FlightAPI.Data; // Cho DbContext
using FlightAPI.Data.Entities; // Cho Banner (Entity)
using FlightAPI.Services.Dtos.Banner; // Cho DTOs
using Microsoft.EntityFrameworkCore; // Cho .Include() và .ToListAsync()
using System.Linq; // Cho .Select()

namespace FlightAPI.Services
{
    // Nó "thề" (implement) 5 "hợp đồng" CRUD
    public class BannerService : IBannerService
    {
        private readonly AirCloudDbContext _context;

        public BannerService(AirCloudDbContext context)
        {
            _context = context;
        }

        // 1. "Đọc" (READ All)
        public async Task<IEnumerable<BannerReadDto>> GetAllAsync()
        {
            return await _context.Banners
                .AsNoTracking()
                // "Tham lam": Vớ (JOIN) luôn "Deal" (Promotion)
                .Include(b => b.Promotion)
                .Select(b => new BannerReadDto
                {
                    BannerId = b.BannerId,
                    Title = b.Title,
                    ImageUrl = b.ImageUrl,
                    LinkUrl = b.LinkUrl,
                    IsActive = (bool)b.IsActive,
                    PromotionId = b.PromotionId,
                    // "In" tên "Deal" (nếu có)
                    PromotionDescription = b.Promotion != null ? b.Promotion.Description : null
                })
                .ToListAsync();
        }

        // 2. "Đọc" (READ One)
        public async Task<BannerReadDto?> GetByIdAsync(int id)
        {
            var b = await _context.Banners
                .Include(b => b.Promotion) // Vớ "Deal"
                .AsNoTracking()
                .FirstOrDefaultAsync(b => b.BannerId == id);

            if (b == null) return null;

            return new BannerReadDto
            {
                BannerId = b.BannerId,
                Title = b.Title,
                ImageUrl = b.ImageUrl,
                LinkUrl = b.LinkUrl,
                IsActive = (bool)b.IsActive,
                PromotionId = b.PromotionId,
                PromotionDescription = b.Promotion != null ? b.Promotion.Description : null
            };
        }

        // 3. "Tạo" (CREATE)
        public async Task<BannerReadDto> CreateAsync(BannerRequestDto dto)
        {
            // (Sếp có 1 cột 'CreatedBy' (int?) trong DB mà "bỏ quên" trong DTO.
            //  Nếu sếp muốn "xịn", sếp phải truyền 'accountId' vào hàm này
            //  và gán: CreatedBy = accountId. 
            //  Giờ, ta "lờ" nó đi cho "nhanh".)

            var newBanner = new Banner
            {
                Title = dto.Title,
                ImageUrl = dto.ImageUrl,
                LinkUrl = dto.LinkUrl,
                StartDate = dto.StartDate ?? DateTime.UtcNow, // Nếu null, lấy giờ hiện tại
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                PromotionId = dto.PromotionId
            };

            _context.Banners.Add(newBanner);
            await _context.SaveChangesAsync();

            // Trả về "biên nhận" (DTO)
            return (await GetByIdAsync(newBanner.BannerId))!; // Gọi "máy in" xịn
        }

        // 4. "Sửa" (UPDATE)
        public async Task<BannerReadDto> UpdateAsync(int id, BannerRequestDto dto)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                throw new KeyNotFoundException("Không tìm thấy banner.");
            }

            // "Đập" (Update) "hàng"
            banner.Title = dto.Title;
            banner.ImageUrl = dto.ImageUrl;
            banner.LinkUrl = dto.LinkUrl;
            banner.StartDate = dto.StartDate ?? banner.StartDate;
            banner.EndDate = dto.EndDate;
            banner.IsActive = dto.IsActive;
            banner.PromotionId = dto.PromotionId;

            await _context.SaveChangesAsync();

            return (await GetByIdAsync(banner.BannerId))!;
        }

        // 5. "Xóa" (DELETE)
        public async Task DeleteAsync(int id)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                throw new KeyNotFoundException("Không tìm thấy banner.");
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
        }
    }
}