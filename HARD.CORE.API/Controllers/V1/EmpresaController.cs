using Asp.Versioning;
using HARD.CORE.API.Controllers.Base;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace HARD.CORE.API.Controllers.V1
{
    /// <summary>
    /// Controller for managing user companies.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for managing user companies.
    /// </remarks>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path
    [ApiController]
    public class EmpresaController : BaseController
    {

        private readonly IEmpresaService _empresaService;

        /// <summary>
        /// Initializes a new instance of the <see cref="EmpresaController"/> class.
        /// </summary>
        /// <param name="empresaService">
        /// The company service.
        /// </param>
        public EmpresaController(IEmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        /// <summary>
        /// Obtains a company by its unique key.
        /// </summary>
        /// <param name="idEmpresa">The unique key identifying the company.</param>
        /// <returns>The company associated with the provided key.</returns>
        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idEmpresa)
        {
            var webResult = await _empresaService.GetByIdAsync(idEmpresa);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Obtains all companies.
        /// </summary>
        /// <param name="idPerfil">
        /// The unique key identifying the profile.
        /// </param>
        /// <param name="claveUsuario">
        /// The unique key identifying the user.
        /// </param>
        /// <param name="estatus">
        /// The status to filter companies.
        /// </param>
        /// <returns>
        /// A list of companies matching the specified criteria.
        /// </returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? activo = null, [FromQuery] int? idPerfil = null, [FromQuery] int? idUsuario = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter filter = new BaseFilter
            {
                Activo = activo,
                IdMaster = idPerfil,
                IdDetail = idUsuario,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            
            var webResult = await _empresaService.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Empresa empresa)
        {
            var webResult = await _empresaService.AddAsync(empresa, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Empresa empresa)
        {
            var webResult = await _empresaService.UpdateAsync(empresa, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idEmpresa)
        {
            var webResult = await _empresaService.DeleteAsync(idEmpresa, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Obtains all companies assigned to a user.
        /// </summary>
        /// <param name="idUsuario">
        /// The unique key identifying the user.
        /// </param>
        /// <returns>
        /// A list of companies assigned to the user.
        /// </returns>
        [HttpGet("GetCompaniesByUser")]
        public async Task<IActionResult> GetCompaniesByUser([FromQuery, Required] int idUsuario, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var webResult = await _empresaService.GetCompaniesByUserAsync(idUsuario, pageIndex, pageSize);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

    }
}