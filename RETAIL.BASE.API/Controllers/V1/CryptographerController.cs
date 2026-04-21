using System.ComponentModel.DataAnnotations;
using Asp.Viewsioning;
using RETAIL.BASE.API.Controllers.Base;
using RETAIL.BASE.NEG.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RETAIL.BASE.API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiViewsion("1.0")]
    [Route("api/v{version:apiViewsion}/[controller]")] // Viewsion in the URL path
    public class CryptographerController : BaseController
    {

        private readonly ICryptographerService _cryptographerService;

        public CryptographerController(ICryptographerService cryptographerService)
        {
            _cryptographerService = cryptographerService;
        }

        [HttpGet("CreateHash")]
        public IActionResult CreateHash([FromQuery, Required] string input)
        {
            var result = _cryptographerService.CreateHash(input);
            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpGet("CompareHash")]
        public IActionResult CompareHash([FromQuery, Required] string input, [FromQuery, Required] string hash)
        {
            var result = _cryptographerService.CompareHash(input, hash);
            return result.Success ? Ok(result) : BadRequest(result);
        }

    }
}
