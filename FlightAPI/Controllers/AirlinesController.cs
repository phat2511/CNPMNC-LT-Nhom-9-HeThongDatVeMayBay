using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FlightAPI.Services;
using FlightAPI.Models;
using System.Linq;

// Sử dụng AirlineCreateDto làm DTO chung cho tất cả các hoạt động CRUD
// Lưu ý: Việc này yêu cầu IAirlineService và AirlineService cũng sử dụng AirlineCreateDto cho Create và Update.

[Route("api/[controller]")]
[ApiController]
public class AirlinesController : ControllerBase
{
    private readonly IAirlineService _airlineService;

    // Dependency Injection: Inject IAirlineService vào Controller
    public AirlinesController(IAirlineService airlineService)
    {
        _airlineService = airlineService;
    }

    // GET: api/Airlines
    [HttpGet]
    public async Task<ActionResult<IEnumerable<AirlineCreateDto>>> GetAll()
    {
        // Lấy tất cả các hãng hàng không
        var airlines = await _airlineService.GetAllAsync();
        return Ok(airlines);
    }

    // GET: api/Airlines/VN (Sử dụng code làm khóa chính)
    [HttpGet("{code}")]
    public async Task<ActionResult<AirlineCreateDto>> GetByCode(string code)
    {
        var airline = await _airlineService.GetByCodeAsync(code);

        if (airline == null)
        {
            return NotFound(); // Trả về 404 nếu không tìm thấy
        }

        return Ok(airline);
    }

    // POST: api/Airlines
    // Sử dụng AirlineCreateDto làm đầu vào (thay vì CreateRequest)
    [HttpPost]
    public async Task<ActionResult<AirlineCreateDto>> Create([FromBody] AirlineCreateDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState); // Trả về 400 Bad Request nếu dữ liệu không hợp lệ
        }

        try
        {
            var newAirline = await _airlineService.CreateAsync(request);

            // Trả về 201 Created và đường dẫn đến tài nguyên mới
            return CreatedAtAction(nameof(GetByCode), new { code = newAirline.AirlineCode }, newAirline);
        }
        catch (InvalidOperationException ex)
        {
            // Xử lý lỗi trùng mã hãng bay (ví dụ: code đã tồn tại)
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi khi tạo hãng hàng không.");
        }
    }

    // PUT: api/Airlines/VN
    // Sử dụng AirlineCreateDto làm đầu vào (thay vì UpdateRequest)
    [HttpPut("{code}")]
    public async Task<IActionResult> Update(string code, [FromBody] AirlineCreateDto request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await _airlineService.UpdateAsync(code, request);
            return NoContent(); // 204 No Content sau khi cập nhật thành công
        }
        catch (KeyNotFoundException)
        {
            return NotFound(); // Trả về 404 nếu không tìm thấy
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi khi cập nhật hãng hàng không.");
        }
    }

    // DELETE: api/Airlines/VN
    [HttpDelete("{code}")]
    public async Task<IActionResult> Delete(string code)
    {
        try
        {
            await _airlineService.DeleteAsync(code);
            return NoContent(); // 204 No Content sau khi xóa thành công
        }
        catch (KeyNotFoundException)
        {
            return NotFound(); // Trả về 404 nếu không tìm thấy
        }
        catch (Exception)
        {
            return StatusCode(500, "Lỗi khi xóa hãng hàng không.");
        }
    }
}
