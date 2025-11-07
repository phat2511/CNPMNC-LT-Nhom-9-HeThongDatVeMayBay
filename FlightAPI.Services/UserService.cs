using FlightAPI.Data; // Cho DbContext
using FlightAPI.Data.Entities; // Cho Account, Role
using FlightAPI.Services.Dtos.User; // Cho DTOs
using Microsoft.EntityFrameworkCore; // Cho .Include() và .ToListAsync()

namespace FlightAPI.Services
{
    // Nó "thề" (implement) 3 "hợp đồng"
    public class UserService : IUserService
    {
        private readonly AirCloudDbContext _context;

        // "Não" này "lười", chỉ cần 1 "đồ nghề": DbContext
        public UserService(AirCloudDbContext context)
        {
            _context = context;
        }

        // 1. "Đọc" (READ All Users)
        public async Task<IEnumerable<UserDetailDto>> GetAllAsync()
        {
            // Vào "kho" Account
            return await _context.Users
                // "Tham lam": Vớ (JOIN) luôn "vai trò" (Role)
                .Include(u => u.Role)
                .AsNoTracking()
                // "Nhào nặn" (Select) thành "Hồ sơ" (DTO)
                .Select(u => new UserDetailDto
                {
                    Id = u.Id,
                    Username = u.UserName,
                    FullName = u.FullName,
                    Email = u.Email,
                    Phone = u.PhoneNumber,
                    RoleName = u.Role.Name, // Lấy "tên" từ "vai trò" đã "vớ"
                    IsLocked = u.IsLocked ?? false, // Xử lý 'bool?' (nullable)
                    IsEmailConfirmed = u.EmailConfirmed,
                    CreatedAt = u.CreatedAt ?? DateTime.MinValue // Xử lý 'datetime?'
                })
                .ToListAsync();
        }

        // 2. "Sửa" (UPDATE - Khóa/Mở)
        public async Task ToggleLockAsync(int userId, UserLockRequestDto dto)
        {
            // 1. Tìm "nhân sự"
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User không tồn tại.");
            }

            // 2. "Ký" lệnh
            user.IsLocked = dto.Lock;
            user.LockedBy = dto.Lock ? "Admin" : null;
            user.LockedAt = dto.Lock ? DateTime.UtcNow : null;
            user.LockReason = dto.Lock ? dto.LockReason : null;

            // 3. "Lưu" (Save)
            await _context.SaveChangesAsync();
        }

        // 3. "Sửa" (UPDATE - Thăng/Giáng chức)
        public async Task ChangeRoleAsync(int userId, UserRoleUpdateDto dto)
        {
            // 1. Tìm "nhân sự"
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException("User không tồn tại.");
            }

            // 2. Tìm "vương miện" (Role) mới
            var newRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.Name == dto.NewRoleName);
            if (newRole == null)
            {
                throw new KeyNotFoundException($"Role '{dto.NewRoleName}' không tồn tại.");
            }

            // 3. "Trao" (Gán RoleId mới)
            user.RoleId = newRole.Id;

            // 4. "Lưu" (Save)
            await _context.SaveChangesAsync();
        }
    }
}