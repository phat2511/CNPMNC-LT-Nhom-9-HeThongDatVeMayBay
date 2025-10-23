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

        public async Task<IEnumerable<FlightInstanceReadDto>> SearchFlightsAsync(
            string depCode, string arrCode, DateTime date)
        {
            // 1. Xây dựng truy vấn
            var query = _context.FlightInstances
                // Yêu cầu EF Core Join các bảng liên quan (Bắt buộc cho Mapping)
                .Include(fi => fi.Flight)
                    .ThenInclude(f => f.AirlineCodeNavigation) // Lấy Tên Hãng
                .Include(fi => fi.DepartureAirportNavigation) // Lấy Thông tin Sân bay Đi
                .Include(fi => fi.ArrivalAirportNavigation)   // Lấy Thông tin Sân bay Đến
                .AsNoTracking(); // Tối ưu cho thao tác đọc (GET)

            // 2. Lọc theo điều kiện (Filtering)
            query = query.Where(fi => fi.DepartureAirport == depCode && fi.ArrivalAirport == arrCode);

            // Lọc theo ngày (Chỉ lấy phần Date, bỏ qua giờ)
            query = query.Where(fi => EF.Property<DateTime>(fi, nameof(fi.DepartureTime)).Date == date.Date);

            // 3. Thực thi truy vấn và ánh xạ
            var results = await query.ToListAsync();
            return _mapper.Map<IEnumerable<FlightInstanceReadDto>>(results);
        }

        public async Task<FlightInstanceReadDto> CreateInstanceAsync(FlightInstanceCreateDto dto)
        {
            var entity = _mapper.Map<FlightInstance>(dto);

            // Logic nghiệp vụ: Kiểm tra DepartureTime có hợp lệ không (Database cũng kiểm tra)
            if (entity.DepartureTime <= DateTime.Now)
                throw new InvalidOperationException("Thời gian khởi hành phải ở tương lai.");

            _context.FlightInstances.Add(entity);
            await _context.SaveChangesAsync();

            // Tải lại Entity sau khi lưu để có các tham chiếu (ví dụ: tên hãng)
            var createdInstance = await _context.FlightInstances
                .Include(fi => fi.Flight)
                    .ThenInclude(f => f.AirlineCodeNavigation)
                .Include(fi => fi.DepartureAirportNavigation)
                .Include(fi => fi.ArrivalAirportNavigation)
                .FirstOrDefaultAsync(fi => fi.FlightInstanceId == entity.FlightInstanceId);

            return _mapper.Map<FlightInstanceReadDto>(createdInstance);
        }

        public async Task<FlightInstanceReadDto?> GetInstanceByIdAsync(int id)
        {
            // Tương tự, dùng Include để lấy đầy đủ thông tin khi GET
            var instance = await _context.FlightInstances
                 .Include(fi => fi.Flight)
                    .ThenInclude(f => f.AirlineCodeNavigation)
                 .Include(fi => fi.DepartureAirportNavigation)
                 .Include(fi => fi.ArrivalAirportNavigation)
                 .FirstOrDefaultAsync(fi => fi.FlightInstanceId == id);

            return _mapper.Map<FlightInstanceReadDto>(instance);
        }

        // ... (Update/Delete)
        public async Task<bool> UpdateInstanceAsync(int id, FlightInstanceCreateDto dto)
        {
            // 1. Tìm Entity hiện tại
            var entityToUpdate = await _context.FlightInstances.FindAsync(id);

            if (entityToUpdate == null)
            {
                return false; // Trả về false nếu không tìm thấy ID
            }

            // 2. Ánh xạ dữ liệu từ DTO vào Entity hiện tại
            // AutoMapper sẽ chỉ cập nhật các thuộc tính mà bạn muốn thay đổi (thời gian, giá vé, v.v.)
            _mapper.Map(dto, entityToUpdate);

            // Đảm bảo khóa ngoại FlightId không bị đổi thành 0 nếu nó được bảo vệ
            // Nếu bạn muốn cho phép đổi FlightId, hãy kiểm tra tính hợp lệ ở đây.

            // 3. Lưu thay đổi
            try
            {
                _context.FlightInstances.Update(entityToUpdate);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                // Xử lý lỗi đồng thời nếu có
                return false;
            }
            catch (Exception ex)
            {
                // Xử lý các lỗi khác (ví dụ: lỗi ràng buộc check constraint)
                // Log lỗi tại đây
                return false;
            }
        }

        public async Task<bool> DeleteInstanceAsync(int id)
        {
            // 1. Tìm Entity
            var entityToDelete = await _context.FlightInstances.FindAsync(id);

            if (entityToDelete == null)
            {
                return false; // Không tìm thấy để xóa
            }

            // 2. Xóa và lưu thay đổi
            _context.FlightInstances.Remove(entityToDelete);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}