using AutoMapper;
using FlightAPI.Data.Entities;
using FlightAPI.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace FlightAPI.Services
{
    public class FlightInstanceService : IFlightInstanceService
    {
        private readonly AirCloudDbContext _context;
        private readonly IMapper _mapper;

        public FlightInstanceService(AirCloudDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // =======================================================
        // ĐÃ THÊM: PHƯƠNG THỨC GET ALL (BỊ THIẾU TRONG CODE CŨ)
        // =======================================================
        public async Task<IEnumerable<FlightInstanceReadDto>> GetAllAsync()
        {
            var instances = await _context.FlightInstances
                .Include(fi => fi.Flight)
                    .ThenInclude(f => f.AirlineCodeNavigation)
                .Include(fi => fi.DepartureAirportNavigation)
                .Include(fi => fi.ArrivalAirportNavigation)
                .AsNoTracking()
                .ToListAsync();

            return _mapper.Map<IEnumerable<FlightInstanceReadDto>>(instances);
        }

        // =======================================================
        // ĐÃ SỬA: TÌM KIẾM (Cho phép tham số tùy chọn)
        // =======================================================
        public async Task<IEnumerable<FlightInstanceReadDto>> SearchFlightsAsync(
            string? depCode, string? arrCode, DateTime? date) // Sửa: Thêm '?' để cho phép null
        {
            var query = _context.FlightInstances
                .Include(fi => fi.Flight)
                    .ThenInclude(f => f.AirlineCodeNavigation)
                .Include(fi => fi.DepartureAirportNavigation)
                .Include(fi => fi.ArrivalAirportNavigation)
                .AsNoTracking();

            // 2. Lọc theo điều kiện (Filtering)
            if (!string.IsNullOrEmpty(depCode)) // Sửa: Thêm kiểm tra null/empty
            {
                query = query.Where(fi => fi.DepartureAirport == depCode);
            }

            if (!string.IsNullOrEmpty(arrCode)) // Sửa: Thêm kiểm tra null/empty
            {
                query = query.Where(fi => fi.ArrivalAirport == arrCode);
            }

            if (date.HasValue) // Sửa: Thêm kiểm tra null
            {
                query = query.Where(fi => fi.DepartureTime.Date == date.Value.Date);
            }

            var results = await query.ToListAsync();
            return _mapper.Map<IEnumerable<FlightInstanceReadDto>>(results);
        }

        public async Task<FlightInstanceReadDto> CreateInstanceAsync(FlightInstanceCreateDto dto)
        {
            var entity = _mapper.Map<FlightInstance>(dto);

            if (entity.DepartureTime <= DateTime.Now)
                throw new InvalidOperationException("Thời gian khởi hành phải ở tương lai.");

            _context.FlightInstances.Add(entity);
            await _context.SaveChangesAsync();

            var createdInstance = await _context.FlightInstances
                .Include(fi => fi.Flight)
                    .ThenInclude(f => f.AirlineCodeNavigation)
                .Include(fi => fi.DepartureAirportNavigation)
                .Include(fi => fi.ArrivalAirportNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(fi => fi.FlightInstanceId == entity.FlightInstanceId); 

            return _mapper.Map<FlightInstanceReadDto>(createdInstance);
        }

        public async Task<FlightInstanceReadDto?> GetInstanceByIdAsync(int id)
        {
            var instance = await _context.FlightInstances
                .Include(fi => fi.Flight)
                    .ThenInclude(f => f.AirlineCodeNavigation)
                .Include(fi => fi.DepartureAirportNavigation)
                .Include(fi => fi.ArrivalAirportNavigation)
                .AsNoTracking()
                .FirstOrDefaultAsync(fi => fi.FlightInstanceId == id); // Sửa: Dùng 'Id'

            return _mapper.Map<FlightInstanceReadDto>(instance);
        }

        public async Task<bool> UpdateInstanceAsync(int id, FlightInstanceCreateDto dto)
        {
            // FindAsync hoạt động trên Khóa chính (Id)
            var entityToUpdate = await _context.FlightInstances.FindAsync(id);

            if (entityToUpdate == null)
            {
                return false;
            }

            _mapper.Map(dto, entityToUpdate);

            try
            {
                // Không cần gọi .Update() khi đã dùng FindAsync (EF Core đang theo dõi)
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteInstanceAsync(int id)
        {
            // FindAsync hoạt động trên Khóa chính (Id)
            var entityToDelete = await _context.FlightInstances.FindAsync(id);

            if (entityToDelete == null)
            {
                return false;
            }

            _context.FlightInstances.Remove(entityToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}