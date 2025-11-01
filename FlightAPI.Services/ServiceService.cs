using FlightAPI.Data; // Cho DbContext
using FlightAPI.Data.Entities; // Cho Service (Entity)
using FlightAPI.Services.Dtos.Service; // Cho DTOs
using Microsoft.EntityFrameworkCore; // Cho .ToListAsync()

namespace FlightAPI.Services
{
    // Nó "thề" (implement) 5 "hợp đồng" CRUD
    public class ServiceService : IServiceService
    {
        private readonly AirCloudDbContext _context;

        // "Tiêm" DbContext vào
        public ServiceService(AirCloudDbContext context)
        {
            _context = context;
        }

        // 1. "Đọc" (READ All)
        public async Task<IEnumerable<ServiceDto>> GetAllAsync()
        {
            // Vào kho, lấy hết, "nhào nặn" thành DTO
            return await _context.Services
                .Select(s => new ServiceDto
                {
                    ServiceId = s.ServiceId,
                    Name = s.Name,
                    Price = (decimal)s.Price,
                    IsActive = (bool)s.IsActive
                })
                .ToListAsync();
        }

        // 2. "Đọc" (READ One)
        public async Task<ServiceDto?> GetByIdAsync(int id)
        {
            // Vào kho, tìm
            var service = await _context.Services.FindAsync(id);

            // Không tìm thấy? Báo "null"
            if (service == null) return null;

            // Thấy? "Nhào nặn" DTO
            return new ServiceDto
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Price = (decimal)service.Price,
                IsActive = (bool)service.IsActive
            };
        }

        // 3. "Tạo" (CREATE)
        public async Task<ServiceDto> CreateAsync(ServiceRequestDto dto)
        {
            // 1. "Đúc" "hàng" (Entity) từ "phiếu" (DTO)
            var newService = new Service
            {
                Name = dto.Name,
                Price = dto.Price,
                IsActive = dto.IsActive
            };

            // 2. "Ném" vào kho
            _context.Services.Add(newService);
            await _context.SaveChangesAsync();

            // 3. Trả về "biên nhận" (DTO)
            return new ServiceDto
            {
                ServiceId = newService.ServiceId,
                Name = newService.Name,
                Price = (decimal)newService.Price,
                IsActive = (bool)newService.IsActive
            };
        }

        // 4. "Sửa" (UPDATE)
        public async Task<ServiceDto> UpdateAsync(int id, ServiceRequestDto dto)
        {
            // 1. Tìm "hàng"
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                // Không thấy "hàng"? Ném "lỗi 404"
                throw new KeyNotFoundException("Không tìm thấy dịch vụ.");
            }

            // 2. "Đập" (Update) "hàng"
            service.Name = dto.Name;
            service.Price = dto.Price;
            service.IsActive = dto.IsActive;

            // 3. Lưu
            await _context.SaveChangesAsync();

            // 4. Trả "biên nhận"
            return new ServiceDto
            {
                ServiceId = service.ServiceId,
                Name = service.Name,
                Price = (decimal)service.Price,
                IsActive = (bool)service.IsActive
            };
        }

        // 5. "Xóa" (DELETE)
        public async Task DeleteAsync(int id)
        {
            // 1. Tìm "hàng"
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                // Không thấy "hàng"? Ném "lỗi 404"
                throw new KeyNotFoundException("Không tìm thấy dịch vụ.");
            }

            // 2. "Hủy" (Remove) "hàng"
            _context.Services.Remove(service);

            // 3. Lưu
            await _context.SaveChangesAsync();

            // (Xóa là xong, không cần trả DTO)
        }
    }
}