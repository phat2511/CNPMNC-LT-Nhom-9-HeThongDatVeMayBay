using FlightAPI.Services;
using FlightAPI.Data.Models;
using Microsoft.AspNetCore.Mvc;

[Route("api/[controller]")] // Route sẽ là: /api/Airports
[ApiController]
public class AirportsController : ControllerBase
{
    private readonly IAirportService _service;

    public AirportsController(IAirportService service)
    {
        _service = service;
    }

    // GET: api/Airports
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var airports = await _service.GetAllAsync();
        return Ok(airports); // Trả về 200 OK
    }

    // GET: api/Airports/SGN
    [HttpGet("{code}")]
    public async Task<IActionResult> GetByCode(string code)
    {
        var airport = await _service.GetByCodeAsync(code);
        if (airport == null) return NotFound(); // Trả về 404 Not Found
        return Ok(airport);
    }

    // POST: api/Airports
    [HttpPost]
    public async Task<IActionResult> Create(AirportCreateDto dto)
    {
        // Kiểm tra validation từ Data Annotations trong DTO
        if (!ModelState.IsValid) return BadRequest(ModelState); // Trả về 400 Bad Request

        var createdAirport = await _service.CreateAsync(dto);
        // Trả về 201 Created
        return CreatedAtAction(nameof(GetByCode), new { code = createdAirport.AirportCode }, createdAirport);
    }

    // PUT: api/Airports/SGN
    [HttpPut("{code}")]
    public async Task<IActionResult> Update(string code, AirportCreateDto dto)
    {
        if (!ModelState.IsValid || code != dto.AirportCode) return BadRequest();

        bool success = await _service.UpdateAsync(code, dto);
        if (!success) return NotFound();

        return NoContent(); // Trả về 204 No Content
    }

    // DELETE: api/Airports/SGN
    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        bool success = await _service.DeleteAsync(code);
        if (!success) return NotFound();
        return NoContent(); // 204 No Content
    }
}