using Asp.Versioning;
using HARD.CORE.API.Controllers.Base;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace HARD.CORE.API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path
    public class ClienteController : BaseController
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? activo = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter filter = new BaseFilter
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Activo = activo
            };
            var webResult = await _clienteService.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idCliente)
        {
            var webResult = await _clienteService.GetByIdAsync(idCliente);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Cliente cliente)
        {
            var webResult = await _clienteService.AddAsync(cliente, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Cliente cliente)
        {
            var webResult = await _clienteService.UpdateAsync(cliente, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idCliente)
        {
            var webResult = await _clienteService.DeleteAsync(idCliente, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }
    }
}
