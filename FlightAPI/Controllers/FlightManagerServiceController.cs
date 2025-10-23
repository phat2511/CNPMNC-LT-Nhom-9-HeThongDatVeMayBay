using FlightAPI.Services;
using Microsoft.AspNetCore.Mvc;
using FlightAPI.Models;

// End of assumed classes

[Route("api/[controller]")]
[ApiController]
public class FlightManagerServiceController : ControllerBase
{
    private readonly IFlightManagerService _flightService;

    public FlightManagerServiceController(IFlightManagerService flightService)
    {
        _flightService = flightService;
    }

    // GET: api/Flights
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var flights = await _flightService.GetAllAsync();
        return Ok(flights);
    }

    // GET: api/Flights/5
    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var flight = await _flightService.GetByIdAsync(id);

        if (flight == null)
        {
            return NotFound();
        }

        return Ok(flight);
    }

    // POST: api/Flights
    [HttpPost]
    public async Task<ActionResult> Create([FromBody] FlightManagerServiceCreateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var newFlight = await _flightService.CreateAsync(request);
        // Trả về 201 Created và đường dẫn đến tài nguyên mới
        return CreatedAtAction(nameof(GetById), new { id = newFlight.Id }, newFlight);
    }

    // PUT: api/Flights/5
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] FlightManagerServiceUpdateRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _flightService.UpdateAsync(id, request);
            return NoContent(); // 204 No Content
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi khi cập nhật chuyến bay.");
        }
    }

    // DELETE: api/Flights/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _flightService.DeleteAsync(id);
            return NoContent(); // 204 No Content
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi khi xóa chuyến bay.");
        }
    }
}
