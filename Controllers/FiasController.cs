using Microsoft.AspNetCore.Mvc;
using FiasApi.Services;

namespace FiasApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FiasController : ControllerBase
    {
        private readonly IFiasService _fiasService;

        public FiasController(IFiasService fiasService)
        {
            _fiasService = fiasService;
        }

        [HttpGet("download")]
        public async Task<IActionResult> Download([FromQuery] int region)
        {
            var result = await _fiasService.DownloadAndExtractAsync(region);
            return Ok(result);
        }

        [HttpGet("process")]
        public async Task<IActionResult> Process([FromQuery] string path, [FromQuery] int region)
        {
            var result = await _fiasService.ExtractFromLocalAsync(path, region);
            return Ok(result);
        }
    }
}
