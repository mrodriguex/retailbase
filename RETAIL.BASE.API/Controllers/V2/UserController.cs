using Asp.Viewsioning;
using RETAIL.BASE.API.Controllers.Base;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.ComponentModel.DataAnnotations;

namespace RETAIL.BASE.API.Controllers.V2
{
    [Authorize]
    [ApiController]
    [ApiViewsion("2.0")]
    [Route("api/v{version:apiViewsion}/[controller]")] // Viewsion in the URL path
    /// <summary>
    /// Controller for user management.
    /// </summary>  
    public class UserController : BaseController
    {

        private readonly IUserService _userservice;
        private readonly IAuthService _authService;
        /// <summary>
        /// Initializes a new instance of the <see cref="UserController"/> class.
        /// </summary>
        /// <param name="userservice">The user service layer.</param>
        /// <param name="authService">The authentication service layer.</param>
        public UserController(IUserService userservice, IAuthService authService)
        {
            _userservice = userservice;
            _authService = authService;
        }
        /// <summary>
        /// Gets user information by user key.
        /// </summary>
        /// <param name="idUser">The unique identifier of the user.</param>
        /// <returns>
        ///     The user information if found; otherwise, an error message.
        /// </returns>
        [HttpGet("GetById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByIdAsync([FromQuery, Required] int idUser)
        {
            var webResult = await _userservice.GetByIdAsync(idUser);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Gets all users.
        /// </summary>   
        /// <param name="enabled">The status filter for users.</param>
        /// <returns>
        ///     A list of all users if found; otherwise, an error message.
        /// </returns>
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll([FromQuery] bool? enabled = null, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            BaseFilter filter = new BaseFilter()
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                Enabled = enabled
            };

            var webResult = await _userservice.GetAllAsync(filter);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Checks if a user exists in the system.
        /// </summary>
        /// <param name="userName">
        ///     The unique identifier of the user.
        /// </param>
        /// <returns>
        /// True if the user exists; otherwise, false.
        /// </returns>
        [AllowAnonymous]
        [HttpGet("Exists")]
        public async Task<IActionResult> Exists([FromQuery, Required] int idUser)
        {
            var webResult = await _userservice.ExistsAsync(idUser);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Gets the detailed activity information for a user.
        /// </summary>
        /// <param name="user">The user information.</param>
        /// <returns>
        ///     The detailed activity information if found; otherwise, an error message.
        /// </returns>
        [HttpPost("Add")]
        public async Task<IActionResult> Add([FromBody] User user)
        {
            var webResult = await _userservice.AddAsync(user, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Updates the user information.
        /// </summary>
        /// <param name="user">
        /// The user information to update.
        /// </param>
        /// <returns>
        /// A result indicating the success or failure of the update operation.
        /// </returns>
        [HttpPut("Update")]
        public async Task<IActionResult> Update([FromBody] User user)
        {
            var webResult = await _userservice.UpdateAsync(user, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="login">The login information.</param>
        /// <returns>
        ///     True if the user is authenticated; otherwise, false.
        /// </returns>
        [HttpPost("AuthenticateUser")]
        public async Task<IActionResult> AuthenticateUser([FromBody] LoginModel login)
        {
            var webResult = await _authService.AuthenticateUserAsync(login, IdUserAutenticado);
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
            var webResult = await _authService.UpdatePasswordAsync(login, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

        /// <summary>
        /// Unlocks a user.
        /// </summary>
        /// <param name="userName">The user's key.</param>
        /// </param>
        /// <returns>
        ///     True if the user is unlocked successfully; otherwise, false.
        /// </returns>
        [HttpPut("UnlockUser")]
        public async Task<IActionResult> UnlockUser([FromBody, Required] int idUser)
        {
            var webResult = await _userservice.UnlockUserAsync(idUser, IdUserAutenticado);
            return webResult.Success ? Ok(webResult) : BadRequest(webResult);
        }

    }
}
