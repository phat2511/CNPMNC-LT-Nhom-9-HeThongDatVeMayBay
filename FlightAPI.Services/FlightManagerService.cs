
using AutoMapper;
using FlightAPI.Data.Entities; // Giả định Flight Entity nằm trong namespace này
using FlightAPI.Models; // Sử dụng DTOs
using Microsoft.EntityFrameworkCore;

namespace FlightAPI.Services
{
    public class FlightManagerService : IFlightManagerService
    {
        private readonly AirCloudDbContext _context; // DbContext
        private readonly IMapper _mapper;

        public FlightManagerService(AirCloudDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // CREATE
        public async Task<FlightManagerServiceDto> CreateAsync(FlightManagerServiceCreateRequest request)
        {
            var flight = _mapper.Map<Flight>(request);
            _context.Flights.Add(flight);
            await _context.SaveChangesAsync();
            return _mapper.Map<FlightManagerServiceDto>(flight);
        }

        // READ ALL
        public async Task<IEnumerable<FlightManagerServiceDto>> GetAllAsync()
        {
            // Sử dụng .AsNoTracking() cho các thao tác đọc để tối ưu hiệu suất
            var flights = await _context.Flights.AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<FlightManagerServiceDto>>(flights);
        }

        // READ BY ID
        public async Task<FlightManagerServiceDto?> GetByIdAsync(int id)
        {
            var flight = await _context.Flights.AsNoTracking().FirstOrDefaultAsync(f => f.FlightId == id);
            return _mapper.Map<FlightManagerServiceDto>(flight);
        }

        // UPDATE
        public async Task UpdateAsync(int id, FlightManagerServiceUpdateRequest request)
        {
            var existingFlight = await _context.Flights.FindAsync(id);

            if (existingFlight == null)
            {
                // Sử dụng KeyNotFoundException để Controller trả về 404 Not Found
                throw new KeyNotFoundException($"Flight with ID {id} not found.");
            }

            // Map request DTO vào Entity hiện có
            _mapper.Map(request, existingFlight);

            // EF Core sẽ tự động theo dõi thay đổi (tracking)
            await _context.SaveChangesAsync();
        }

        // DELETE
        public async Task DeleteAsync(int id)
        {
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
            {
                throw new KeyNotFoundException($"Flight with ID {id} not found.");
            }

            _context.Flights.Remove(flight);
            await _context.SaveChangesAsync();
        }
    }
}
