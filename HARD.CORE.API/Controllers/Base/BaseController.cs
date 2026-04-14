using Asp.Versioning;
using HARD.CORE.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HARD.CORE.API.Controllers.Base
{
    [Authorize]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path

    public abstract class BaseController : ControllerBase
    {

        /// <summary>
        /// Gets the unique key identifying the user from the JWT token.
        /// </summary>
        public int IdUsuarioAutenticado
        {
            get
            {
                string? token = Request.Headers["Authorization"];
                return JwtAuthenticateHelper.GetUserIdFromToken(token ?? "");
            }
        }

    }

}

