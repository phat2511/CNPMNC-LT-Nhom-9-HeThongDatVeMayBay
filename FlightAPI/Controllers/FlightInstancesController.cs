using FlightAPI.Services;
using FlightAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")] 
[ApiController]
public class FlightInstancesController : ControllerBase
{
    private readonly IFlightInstanceService _service;

    private readonly IFlightInstanceService _flightInstanceService;

    public FlightInstancesController(IFlightInstanceService service, IFlightInstanceService flightInstanceService)
    {
        _service = service;
        _flightInstanceService = flightInstanceService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var instances = await _service.GetAllAsync();
        return Ok(instances);
    }

    // GET: api/Flights/Search?depCode=SGN&arrCode=HAN&date=2025-12-01
    // GET: api/FlightInstances/Search
    [HttpGet("Search")]
    public async Task<IActionResult> SearchFlights(
        [FromQuery] string? depCode, // Sửa: Thêm '?'
        [FromQuery] string? arrCode, // Sửa: Thêm '?'
        [FromQuery] DateTime? date) // Sửa: Thêm '?'
    {
        // Logic này sẽ gọi Service (đã hỗ trợ nullable)
        var flights = await _service.SearchFlightsAsync(depCode, arrCode, date);
        return Ok(flights);
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var instance = await _service.GetInstanceByIdAsync(id);
        if (instance == null) return NotFound();
        return Ok(instance);
    }


    [HttpPost]
    public async Task<IActionResult> Create([FromBody] FlightInstanceCreateDto dto)
    {
        // Validation sẽ được thực hiện bởi Data Annotations (nếu có) và Service Layer
        var createdInstance = await _service.CreateInstanceAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdInstance.FlightInstanceId }, createdInstance);
    }

    // PUT: api/Flights/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FlightInstanceCreateDto dto)
    {
        // Ràng buộc 1: Kiểm tra ModelState (Data Annotations)
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Ràng buộc 2: Kiểm tra ID trong URL và ID của đối tượng (dù DTO không có ID, nhưng tốt hơn là kiểm tra)
        // Vì DTO không có ID, chúng ta chỉ cần kiểm tra logic trong Service

        bool success = await _service.UpdateInstanceAsync(id, dto);

        if (!success)
        {
            // Trả về 404 nếu Service báo không tìm thấy hoặc lỗi Concurrency
            return NotFound();
        }

        return NoContent(); // 204 No Content (Thành công, không cần trả về nội dung)
    }

    // DELETE: api/Flights/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool success = await _service.DeleteInstanceAsync(id);

        if (!success)
        {
            return NotFound(); // 404 nếu không tìm thấy chuyến bay để xóa
        }

        return NoContent(); // 204 No Content (Xóa thành công)
    }

    [HttpGet("{id}/seats")] // <--- API "XEM"
    public async Task<IActionResult> GetSeatMap(int id)
    {
        var seats = await _flightInstanceService.GetSeatsForFlightAsync(id);
        if (seats == null || !seats.Any())
        {
            return NotFound("Không tìm thấy chuyến bay hoặc chuyến bay không có ghế.");
        }
        return Ok(seats);
    }
}