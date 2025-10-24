using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FlightAPI.Data; // Giả định AirCloudDbContext nằm trong đây
using FlightAPI.Data.Entities; // Giả định Airline Entity nằm trong đây
using FlightAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FlightAPI.Services
{
    public class AirlineService : IAirlineService
    {
        private readonly AirCloudDbContext _context;
        private readonly IMapper _mapper;

        public AirlineService(AirCloudDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // CREATE
        public async Task<AirlineCreateDto> CreateAsync(AirlineCreateDto request)
        {
            // Kiểm tra xem AirlineCode đã tồn tại chưa
            if (await _context.Airlines.AnyAsync(a => a.AirlineCode == request.AirlineCode))
            {
                throw new InvalidOperationException($"Airline Code '{request.AirlineCode}' already exists.");
            }

            var airline = _mapper.Map<Airline>(request);
            _context.Airlines.Add(airline);
            await _context.SaveChangesAsync();
            return _mapper.Map<AirlineCreateDto>(airline);
        }

        // READ ALL
        public async Task<IEnumerable<AirlineCreateDto>> GetAllAsync()
        {
            var airlines = await _context.Airlines.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<AirlineCreateDto>>(airlines);
        }

        // READ BY CODE (PK là AirlineCode)
        public async Task<AirlineCreateDto?> GetByCodeAsync(string code)
        {
            var airline = await _context.Airlines.AsNoTracking().FirstOrDefaultAsync(a => a.AirlineCode == code);
            return _mapper.Map<AirlineCreateDto>(airline);
        }

        // UPDATE
        public async Task UpdateAsync(string code, AirlineCreateDto request)
        {
            var existingAirline = await _context.Airlines.FindAsync(code);

            if (existingAirline == null)
            {
                throw new KeyNotFoundException($"Airline with code '{code}' not found.");
            }

            // Chỉ cập nhật Name (vì Code là PK)
            existingAirline.Name = request.Name;

            // EF Core đang theo dõi, chỉ cần SaveChanges
            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteAsync(string code)
        {
            var airline = await _context.Airlines.FindAsync(code);

            if (airline == null)
            {
                throw new KeyNotFoundException($"Airline with code '{code}' not found.");
            }

            // Kiểm tra ràng buộc khóa ngoại (ví dụ: Flight đang dùng Airline này)
            // Nếu có Flight đang tham chiếu, bạn cần xử lý: hoặc cấm xóa, hoặc xóa cascade.
            // Để đơn giản, ta chỉ xóa:
            _context.Airlines.Remove(airline);
            await _context.SaveChangesAsync();
        }
    }
}
