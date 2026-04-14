using Asp.Versioning;
using HARD.CORE.API.Controllers.Base;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace HARD.CORE.API.Controllers.V1
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path
    /// <summary>
    /// Controller for user management.
    /// </summary>  
    public class UsuarioController : BaseController
    {

        private readonly IUsuarioService _usuarioService;
        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the <see cref="UsuarioController"/> class.
        /// </summary>
        /// <param name="usuarioService">The user service layer.</param>
        public UsuarioController(IUsuarioService usuarioService, IAuthService authService)
        {
            _usuarioService = usuarioService;
            _authService = authService;
        }
        /// <summary>
        /// Gets user information by user key.
        /// </summary>
        /// <param name="idUsuario">The unique identifier of the user.</param>
        /// <returns>
        ///     The user information if found; otherwise, an error message.
        /// </returns>
        [HttpGet("GetById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idUsuario)
        {
            var webResult = await _usuarioService.GetByIdAsync(idUsuario);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>   
        /// <param name="activo">The status filter for users.</param>
        /// <returns>
        ///     A list of all users if found; otherwise, an error message.
        /// </returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? activo = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter baseFilter = new BaseFilter()
            {
                Activo = activo,
                PageIndex = pageIndex,
                PageSize = pageSize
            };
            var webResult = await _usuarioService.GetAllAsync(baseFilter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Checks if a user exists in the system.
        /// </summary>
        /// <param name="idUsuario">
        ///     The unique identifier of the user.
        /// </param>
        /// <returns>
        /// True if the user exists; otherwise, false.
        /// </returns>
        [AllowAnonymous]
        [HttpGet("Exists")]
        public async Task<IActionResult> Exists([FromQuery, Required] int idUsuario)
        {
            var webResult = await _usuarioService.ExistsAsync(idUsuario);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Gets the detailed activity information for a user.
        /// </summary>
        /// <param name="usuario">The user information.</param>
        /// <returns>
        ///     The detailed activity information if found; otherwise, an error message.
        /// </returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Usuario usuario)
        {
            var webResult = await _usuarioService.AddAsync(usuario, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Updates the user information.
        /// </summary>
        /// <param name="usuario">
        /// The user information to update.
        /// </param>
        /// <returns>
        /// A result indicating the success or failure of the update operation.
        /// </returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Usuario usuario)
        {
            var webResult = await _usuarioService.UpdateAsync(usuario, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idUsuario)
        {
            var webResult = await _usuarioService.DeleteAsync(idUsuario, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Updates the user's password.
        /// </summary>
        /// <param name="login">The login information.</param>
        /// <returns>
        ///     True if the password is updated successfully; otherwise, false.
        /// </returns>
        /// 
        [HttpPut("UpdatePassword")]
        public async Task<IActionResult> UpdatePassword([FromBody] LoginModel login)
        {
            var webResult = await _authService.UpdatePasswordAsync(login, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Unlocks a user.
        /// </summary>
        /// <param name="claveUsuario">The user's key.</param>
        /// </param>
        /// <returns>
        ///     True if the user is unlocked successfully; otherwise, false.
        /// </returns>
        [HttpPut("UnlockUser")]
        public async Task<IActionResult> UnlockUser([FromBody, Required] int idUsuario)
        {
            var webResult = await _usuarioService.UnlockUserAsync(idUsuario, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

    }
}
