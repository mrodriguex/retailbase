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
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path
    [ApiController]
    public class MenuController : BaseController
    {
        private readonly IMenuService _menuService;

        public MenuController(IMenuService menuService)
        {
            _menuService = menuService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idMenu)
        {
            var webResult = await _menuService.GetByIdAsync(idMenu);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
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

            var webResult = await _menuService.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Inserts a new menu.
        /// </summary>
        /// <param name="menu">The menu to insert.</param>
        /// <returns>The unique key of the inserted menu.</returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Menu menu)
        {
            var webResult = await _menuService.AddAsync(menu, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Updates an existing menu.
        /// </summary>
        /// <param name="menu">The menu to update.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Menu menu)
        {
            var webResult = await _menuService.UpdateAsync(menu, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idMenu)
        {
            var webResult = await _menuService.DeleteAsync(idMenu, IdUsuarioAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        // Obtener menú de usuario
        [HttpGet("GetMenusByUser")]
        public async Task<IActionResult> GetMenusByUser([FromQuery, Required] int idUsuario, [FromQuery, Required] int idPerfil)
        {
            var webResult = await _menuService.GetMenusByUserAsync(idUsuario, idPerfil);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        // Obtener menú de perfil
        [HttpGet("GetMenusByProfile")]
        public async Task<IActionResult> GetMenusByProfile([FromQuery, Required] int idPerfil)
        {
            var webResult = await _menuService.GetMenusByProfileAsync(idPerfil);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

    }
}
