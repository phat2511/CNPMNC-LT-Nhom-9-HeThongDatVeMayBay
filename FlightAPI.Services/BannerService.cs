using FlightAPI.Data;
using FlightAPI.Data.Entities;
using FlightAPI.Services.Dtos.Banner;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.IO; // Cần thiết cho Stream

namespace FlightAPI.Services
{
    public class BannerService : IBannerService
    {
        private readonly AirCloudDbContext _context;
        // === SỬA LỖI 1: KHAI BÁO FIELD VÀ INJECT SERVICE ===
        private readonly IStorageService _storageService;

        // SỬA LỖI 1: Thêm IStorageService vào Constructor
        public BannerService(AirCloudDbContext context, IStorageService storageService)
        {
            _context = context;
            _storageService = storageService;
        }

        // 1. "Đọc" (READ All) - (Giữ nguyên)
        public async Task<IEnumerable<BannerReadDto>> GetAllAsync()
        {
            return await _context.Banners
                .AsNoTracking()
                .Include(b => b.Promotion)
                .Select(b => new BannerReadDto
                {
                    BannerId = b.BannerId,
                    Title = b.Title,
                    ImageUrl = b.ImageUrl,
                    LinkUrl = b.LinkUrl,
                    IsActive = (bool)b.IsActive,
                    PromotionId = b.PromotionId,
                    PromotionDescription = b.Promotion != null ? b.Promotion.Description : null
                })
                .ToListAsync();
        }

        // 2. "Đọc" (READ One) - (Giữ nguyên)
        public async Task<BannerReadDto?> GetByIdAsync(int id)
        {
            var b = await _context.Banners
                .Include(b => b.Promotion)
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
            string imageUrl = null!;

            // 1. UPLOAD FILE
            using (var stream = dto.File.OpenReadStream())
            {
                imageUrl = await _storageService.UploadFileAsync(
                    stream,
                    dto.File.FileName,
                    dto.File.ContentType
                );
            }

            // 2. TẠO ENTITY (Đã sửa lỗi trùng lặp và sử dụng imageUrl đã upload)
            var newBannerEntity = new Banner
            {
                Title = dto.Title,
                ImageUrl = imageUrl, // <--- LƯU URL TỪ CLOUDINARY
                LinkUrl = dto.LinkUrl,
                StartDate = dto.StartDate ?? DateTime.UtcNow,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive,
                PromotionId = dto.PromotionId
            };

            _context.Banners.Add(newBannerEntity);
            await _context.SaveChangesAsync();

            // 3. Trả về "biên nhận"
            return (await GetByIdAsync(newBannerEntity.BannerId))!;
        }

        // 4. "Sửa" (UPDATE)
        public async Task<BannerReadDto> UpdateAsync(int id, BannerRequestDto dto)
        {
            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                throw new KeyNotFoundException("Không tìm thấy banner.");
            }

            string newImageUrl = banner.ImageUrl; // Giữ nguyên URL cũ nếu không có file mới

            // NẾU CÓ FILE MỚI ĐƯỢC GỬI LÊN
            if (dto.File != null && dto.File.Length > 0)
            {
                // 1. Xóa file cũ khỏi Cloudinary
                if (!string.IsNullOrEmpty(banner.ImageUrl))
                {
                    await _storageService.DeleteFileAsync(banner.ImageUrl);
                }

                // 2. Upload file mới
                using (var stream = dto.File.OpenReadStream())
                {
                    newImageUrl = await _storageService.UploadFileAsync(
                        stream, dto.File.FileName, dto.File.ContentType
                    );
                }
            }

            // 3. CẬP NHẬT ENTITY VỚI LOGIC PATCH (chỉ update trường được gửi)
            if (!string.IsNullOrEmpty(dto.Title)) banner.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.LinkUrl)) banner.LinkUrl = dto.LinkUrl;

            // Cập nhật URL (dùng URL mới nếu đã upload, hoặc giữ nguyên URL cũ)
            banner.ImageUrl = newImageUrl;

            // Cập nhật các trường nullable khác
            banner.StartDate = dto.StartDate ?? banner.StartDate;
            banner.EndDate = dto.EndDate;

            // Cập nhật các trường không nullable/bool
            banner.IsActive = dto.IsActive; // (Giả định DTO được sửa thành bool? hoặc bạn đang dùng logic PATCH đơn giản)
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

            // === XÓA FILE TRƯỚC ===
            if (!string.IsNullOrEmpty(banner.ImageUrl))
            {
                await _storageService.DeleteFileAsync(banner.ImageUrl);
            }

            _context.Banners.Remove(banner);
            await _context.SaveChangesAsync();
        }
    }
}