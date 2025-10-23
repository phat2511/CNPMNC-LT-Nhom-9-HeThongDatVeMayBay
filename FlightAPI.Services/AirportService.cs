// FlightAPI.Services/AirportService.cs

using AutoMapper;
using FlightAPI.Data.Entities;
using FlightAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightAPI.Services
{

    public class AirportService : IAirportService
    {
        private readonly AirCloudDbContext _context;
        private readonly IMapper _mapper;

        public AirportService(AirCloudDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AirportReadDto>> GetAllAsync()
        {
            var airports = await _context.Airports.ToListAsync();
            // Ánh xạ từ List<Airport> sang List<AirportReadDto>
            return _mapper.Map<IEnumerable<AirportReadDto>>(airports);
        }

        public async Task<AirportReadDto?> GetByCodeAsync(string code)
        {
            var airport = await _context.Airports
                .AsNoTracking() // Không theo dõi thay đổi (tối ưu cho GET)
                .FirstOrDefaultAsync(a => a.AirportCode == code);

            return _mapper.Map<AirportReadDto>(airport);
        }

        public async Task<AirportReadDto> CreateAsync(AirportCreateDto dto)
        {
            // Ánh xạ từ DTO sang Entity
            var entity = _mapper.Map<Airport>(dto);

            _context.Airports.Add(entity);
            await _context.SaveChangesAsync();

            // Trả về DTO
            return _mapper.Map<AirportReadDto>(entity);
        }



        public async Task<bool> DeleteAsync(string code)
        {
            var entity = await _context.Airports.FindAsync(code);
            if (entity == null) return false;

            _context.Airports.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        // ... Triển khai UpdateAsync ...
        public async Task<bool> UpdateAsync(string code, AirportCreateDto dto)
        {
            // 1. Tìm Entity Airport hiện tại trong Database
            var entityToUpdate = await _context.Airports.FindAsync(code);

            if (entityToUpdate == null)
            {
                return false; // Trả về false nếu không tìm thấy sân bay theo mã code
            }

            // 2. Ánh xạ (Map) dữ liệu từ DTO vào Entity hiện tại.
            // AutoMapper sẽ chỉ cập nhật các thuộc tính có trong DTO (tên, thành phố, quốc gia).
            _mapper.Map(dto, entityToUpdate);

            // 3. Đánh dấu Entity là đã được sửa đổi và lưu vào DB
            _context.Airports.Update(entityToUpdate);

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý lỗi nếu có ai đó đã thay đổi dữ liệu trước đó (Concurrency)
                return false;
            }
        }
    }
}