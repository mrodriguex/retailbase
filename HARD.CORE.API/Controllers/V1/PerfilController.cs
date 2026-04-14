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
    /// Controller for managing user profiles.
    /// </summary>
    /// <remarks>
    /// This controller provides endpoints for managing user profiles.
    /// </remarks>
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path
    [ApiController]
    public class PerfilController : BaseController
    {

        private readonly IPerfilService _perfilService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PerfilController"/> class.
        /// </summary>
        /// <param name="perfilService">
        /// The profile service layer.
        /// </param>
        public PerfilController(IPerfilService perfilService)
        {
            _perfilService = perfilService;
        }

        /// <summary>
        /// Obtains a profile by its unique key.
        /// </summary>
        /// <param name="idPerfil">The unique key identifying the profile.</param>
        /// <returns>The profile associated with the provided key.</returns>
        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idPerfil)
        {
            var webResult = await _perfilService.GetByIdAsync(idPerfil);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Obtains all profiles.
        /// </summary>
        /// <returns>A list of all profiles.</returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? activo = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter filter = new BaseFilter
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Activo = activo
            };

            var webResult = await _perfilService.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }
        /// <summary>
        /// Inserts a new profile.
        /// </summary>
        /// <param name="perfil">The profile to insert.</param>
        /// <returns>The unique key of the inserted profile.</returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Perfil perfil)
        {
            var webResult = await _perfilService.AddAsync(perfil, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Updates an existing profile.
        /// </summary>
        /// <param name="perfil">The profile to update.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Perfil perfil)
        {
            var webResult = await _perfilService.UpdateAsync(perfil, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idPerfil)
        {
            var webResult = await _perfilService.DeleteAsync(idPerfil, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Obtains all profiles assigned to a specific user.
        /// </summary>
        /// <param name="idUsuario">The unique key identifying the user.</param>
        /// <returns>A list of profiles assigned to the specified user.</returns>
        [HttpGet("GetUserProfiles")]
        public async Task<IActionResult> GetUserProfiles([FromQuery, Required] int idUsuario)
        {
            var webResult = await _perfilService.GetUserProfilesAsync(idUsuario);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

    }
}
