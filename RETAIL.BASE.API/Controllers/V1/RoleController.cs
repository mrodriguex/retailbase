using Asp.Versioning;
using RETAIL.BASE.API.Controllers.Base;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace RETAIL.BASE.API.Controllers.V1
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
    public class RoleController : BaseController
    {

        private readonly IRoleService _roleService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RoleController"/> class.
        /// </summary>
        /// <param name="roleService">
        /// The profile service layer.
        /// </param>
        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Obtains a profile by its unique key.
        /// </summary>
        /// <param name="idRole">The unique key identifying the profile.</param>
        /// <returns>The profile associated with the provided key.</returns>
        [HttpGet("GetById")]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idRole)
        {
            var webResult = await _roleService.GetByIdAsync(idRole);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Obtains all profiles.
        /// </summary>
        /// <returns>A list of all profiles.</returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? enabled = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter filter = new BaseFilter
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Enabled = enabled
            };

            var webResult = await _roleService.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }
        /// <summary>
        /// Inserts a new profile.
        /// </summary>
        /// <param name="role">The profile to insert.</param>
        /// <returns>The unique key of the inserted profile.</returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] Role role)
        {
            var webResult = await _roleService.AddAsync(role, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Updates an existing profile.
        /// </summary>
        /// <param name="role">The profile to update.</param>
        /// <returns>True if the update was successful; otherwise, false.</returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] Role role)
        {
            var webResult = await _roleService.UpdateAsync(role, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        [HttpDelete("Delete")]
        public async Task<IActionResult> Delete([FromQuery, Required] int idRole)
        {
            var webResult = await _roleService.DeleteAsync(idRole, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Obtains all profiles assigned to a specific user.
        /// </summary>
        /// <param name="idUser">The unique key identifying the user.</param>
        /// <returns>A list of profiles assigned to the specified user.</returns>
        [HttpGet("GetUserProfiles")]
        public async Task<IActionResult> GetUserProfiles([FromQuery, Required] int idUser)
        {
            var webResult = await _roleService.GetUserProfilesAsync(idUser);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

    }
}
