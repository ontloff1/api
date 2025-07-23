using Microsoft.AspNetCore.Mvc;
using FiasApi.Services;

namespace FiasApi.Controllers;

[ApiController]
[Route("fias")]
public class FiasController : ControllerBase
{
    private readonly FiasService _service;

    public FiasController(FiasService service)
    {
        _service = service;
    }

    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("FIAS API работает!");
    }

    [HttpPost("process/{regionCode}")]
public async Task<IActionResult> ProcessRegion(string regionCode, [FromQuery] bool nextDay = false)
{
    try
    {
        var count = await _service.LoadRegionAsync(regionCode, nextDay);
        return Ok(new { message = $"Загружено {count} записей для региона {regionCode}" });
    }
    catch (Exception ex)
    {
        return BadRequest(new { error = ex.Message });
    }
}
        }
    }
}
