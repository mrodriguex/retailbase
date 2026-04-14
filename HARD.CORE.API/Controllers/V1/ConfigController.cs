using Asp.Versioning;

using Microsoft.AspNetCore.Mvc;

namespace HARD.CORE.API.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path
    public class ConfigController : Controller
    {
        private readonly ConfigService _configService;

        public ConfigController(ConfigService configService)
        {
            _configService = configService;
        }

        // PUT: api/config/{key}
        [HttpPut("{key}")]
        public IActionResult UpdateAppSetting(string key, [FromQuery] string newValue)
        {
            var result = _configService.UpdateAppSetting(key, newValue);
            if (result.Success)
            {
                return Ok(new { message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }

        // GET: api/config/{key}
        [HttpGet("{key}")]
        public IActionResult GetAppSetting(string key)
        {
            var result = _configService.GetAppSetting(key);
            if (result.Success)
            {
                return Ok(new { value = result.Data, message = result.Message });
            }
            return BadRequest(new { message = result.Message });
        }
    }
}
