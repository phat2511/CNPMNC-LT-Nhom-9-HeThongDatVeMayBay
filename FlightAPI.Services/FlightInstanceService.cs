using AutoMapper;
using FlightAPI.Data.Entities;
using FlightAPI.Data.Models;
using FlightAPI.Services.Dtos.Seat;
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

            // === BƯỚC NÂNG CẤP 1: "GỌI ĐẺ" ===
            // Trước khi "thêm" (Add), "bắt" nó "đẻ" ghế (Seat)
            GenerateDummySeats(entity); // "entity" sẽ được "nhồi" đầy ghế

            // === BƯỚC NÂNG CẤP 2: LƯU 1 LẦN ===
            // "Thêm" (Add) "thằng" cha (FlightInstance)
            // EF Core "thông minh", nó sẽ "tự động" "thêm" (Add) luôn 15 "đứa con" (Seats)
            _context.FlightInstances.Add(entity);

            // "Lưu" (Save) 1 lần, 16 "mạng" (1 cha, 15 con) sẽ được INSERT vào DB
            await _context.SaveChangesAsync();

            // (Phần 're-query' của sếp giữ nguyên, rất "chuẩn"...)
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

        public async Task<IEnumerable<SeatDto>> GetSeatsForFlightAsync(int flightInstanceId)
        {
            var seats = await _context.Seats
                // 2. Lọc đúng chuyến bay
                .Where(s => s.FlightInstanceId == flightInstanceId)
                // 3. "Tham lam": Vớ luôn thông tin của Hạng ghế (SeatClass)
                .Include(s => s.SeatClass)
                // 4. "Nhào nặn" (Select) nó thành cái DTO "sạch sẽ"
                .Select(s => new SeatDto
                {
                    SeatId = s.SeatId,
                    SeatNumber = s.SeatNumber,
                    IsAvailable = (bool)s.IsAvailable,
                    // Lấy "hàng" từ bảng SeatClass đã "vớ"
                    SeatClassName = s.SeatClass.Name,
                    PriceMultiplier = (decimal)s.SeatClass.PriceMultiplier
                })
                .ToListAsync();
                    return seats;
        }

        private void GenerateDummySeats(FlightInstance newInstance)
        {
            // (Hàm này "giả lập" việc "đẻ" ghế theo sơ đồ máy bay)
            // (Sếp phải đảm bảo SeatClassId=1 (Economy) và 3 (Business) là có thật!)

            // Đẻ 10 ghế Economy (A01-A05, B01-B05)
            for (int i = 1; i <= 5; i++)
            {
                // (Giả sử Entity 'FlightInstance' có ICollection<Seat> Seats)
                newInstance.Seats.Add(new Seat
                {
                    SeatNumber = $"A0{i}",
                    SeatClassId = 1, // Economy
                    IsAvailable = true
                });
                newInstance.Seats.Add(new Seat
                {
                    SeatNumber = $"B0{i}",
                    SeatClassId = 1, // Economy
                    IsAvailable = true
                });
            }

            // Đẻ 5 ghế Business (F01-F05)
            for (int i = 1; i <= 5; i++)
            {
                newInstance.Seats.Add(new Seat
                {
                    SeatNumber = $"F0{i}",
                    SeatClassId = 3, // Business
                    IsAvailable = true
                });
            }

            // (Không cần Add vào context, EF Core tự "thấy" 15 "đứa con" này
            //  khi sếp "Add" "thằng" cha 'newInstance')
        }
    }
}