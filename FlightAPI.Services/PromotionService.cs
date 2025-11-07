using FlightAPI.Data; // Cho DbContext
using FlightAPI.Data.Entities; // Cho Promotion (Entity)
using FlightAPI.Services.Dtos.Promotion; // Cho DTOs
using Microsoft.EntityFrameworkCore; // Cho .ToListAsync()
using System.Linq; // Cho .Select()

namespace FlightAPI.Services
{
    // Nó "thề" (implement) 5 "hợp đồng" CRUD
    public class PromotionService : IPromotionService
    {
        private readonly AirCloudDbContext _context;

        public PromotionService(AirCloudDbContext context)
        {
            _context = context;
        }

        // 1. "Đọc" (READ All)
        public async Task<IEnumerable<PromotionReadDto>> GetAllAsync()
        {
            return await _context.Promotions
                .AsNoTracking()
                .Select(p => new PromotionReadDto
                {
                    PromotionId = p.PromotionId,
                    Code = p.Code,
                    Description = p.Description,
                    DiscountPercent = p.DiscountPercent,
                    DiscountAmount = p.DiscountAmount,
                    // Giả sử Entity (Promotion) dùng DateOnly? (vì DB là DATE)
                    // Nếu Entity dùng DateTime?, sếp đổi thành:
                    // StartDate = p.StartDate.HasValue ? DateOnly.FromDateTime(p.StartDate.Value) : null,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    IsActive = (bool)p.IsActive
                })
                .ToListAsync();
        }

        // 2. "Đọc" (READ One)
        public async Task<PromotionReadDto?> GetByIdAsync(int id)
        {
            var p = await _context.Promotions.FindAsync(id);
            if (p == null) return null;

            return new PromotionReadDto
            {
                PromotionId = p.PromotionId,
                Code = p.Code,
                Description = p.Description,
                DiscountPercent = p.DiscountPercent,
                DiscountAmount = p.DiscountAmount,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                IsActive = (bool)p.IsActive
            };
        }

        // 3. "Tạo" (CREATE)
        public async Task<PromotionReadDto> CreateAsync(PromotionRequestDto dto)
        {
            // 1. "Đúc" "hàng" (Entity) từ "phiếu" (DTO)
            var newPromotion = new Promotion
            {
                Code = dto.Code,
                Description = dto.Description,
                DiscountPercent = dto.DiscountPercent,
                DiscountAmount = dto.DiscountAmount,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                IsActive = dto.IsActive
            };

            // 2. "Ném" vào kho
            _context.Promotions.Add(newPromotion);
            await _context.SaveChangesAsync();

            // 3. Trả về "biên nhận" (DTO)
            // (Gọi 'GetByIdAsync' để "in" DTO, tránh lặp code)
            return (await GetByIdAsync(newPromotion.PromotionId))!;
        }

        // 4. "Sửa" (UPDATE)
        public async Task<PromotionReadDto> UpdateAsync(int id, PromotionRequestDto dto)
        {
            // 1. Tìm "hàng"
            var p = await _context.Promotions.FindAsync(id);
            if (p == null)
            {
                throw new KeyNotFoundException("Không tìm thấy khuyến mãi.");
            }

            // 2. "Đập" (Update) "hàng"
            p.Code = dto.Code;
            p.Description = dto.Description;
            p.DiscountPercent = dto.DiscountPercent;
            p.DiscountAmount = dto.DiscountAmount;
            p.StartDate = dto.StartDate;
            p.EndDate = dto.EndDate;
            p.IsActive = dto.IsActive;

            // 3. Lưu
            await _context.SaveChangesAsync();

            // 4. Trả "biên nhận"
            return (await GetByIdAsync(p.PromotionId))!;
        }

        // 5. "Xóa" (DELETE)
        public async Task DeleteAsync(int id)
        {
            // 1. Tìm "hàng"
            var p = await _context.Promotions.FindAsync(id);
            if (p == null)
            {
                throw new KeyNotFoundException("Không tìm thấy khuyến mãi.");
            }

            // 2. Check "ràng buộc" (nếu sếp cẩn thận)
            // (Ví dụ: Đơn hàng (Booking) nào đang 'dùng' mã này?)

            // 3. "Hủy" (Remove) "hàng"
            _context.Promotions.Remove(p);

            // 4. Lưu
            await _context.SaveChangesAsync();
        }
    }
}