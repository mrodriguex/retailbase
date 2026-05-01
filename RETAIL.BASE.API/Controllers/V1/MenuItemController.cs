using Asp.Versioning;
using RETAIL.BASE.API.Controllers.Base;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace RETAIL.BASE.API.Controllers.V1
{
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path
    [ApiController]
    public class MenuItemController : BaseController
    {
        private readonly IMenuItemService _menuitemService;

        public MenuItemController(IMenuItemService menuitemService)
        {
            _menuitemService = menuitemService;
        }

        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idMenuItem)
        {
            var webResult = await _menuitemService.GetByIdAsync(idMenuItem);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? enabled = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter filter = new BaseFilter
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Enabled = enabled
            };

            var webResult = await _menuitemService.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Inserts a new menuitem.
        /// </summary>
        /// <param name="menuitem">The menuitem to insert.</param>
        /// <returns>The unique key of the inserted menuitem.</returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] MenuItem menuitem)
        {
            var webResult = await _menuitemService.AddAsync(menuitem, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Updates an existing menuitem.
        /// </summary>
        /// <param name="menuitem">The menuitem to update.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] MenuItem menuitem)
        {
            var webResult = await _menuitemService.UpdateAsync(menuitem, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idMenuItem)
        {
            var webResult = await _menuitemService.DeleteAsync(idMenuItem, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        // Obtener menú de user
        [HttpGet("GetMenuItemsByUser")]
        public async Task<IActionResult> GetMenuItemsByUser([FromQuery, Required] int idUser, [FromQuery, Required] int idRole)
        {
            var webResult = await _menuitemService.GetMenuItemsByUserAsync(idUser, idRole);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        // Obtener menú de role
        [HttpGet("GetMenuItemsByProfile")]
        public async Task<IActionResult> GetMenuItemsByProfile([FromQuery, Required] int idRole)
        {
            var webResult = await _menuitemService.GetMenuItemsByProfileAsync(idRole);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

    }
}
