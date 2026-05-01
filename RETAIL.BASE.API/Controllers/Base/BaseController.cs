using Asp.Versioning;
using RETAIL.BASE.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RETAIL.BASE.API.Controllers.Base
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
        public int IdUserAutenticado
        {
            get
            {
                string? token = Request.Headers["Authorization"];
                return JwtAuthenticateHelper.GetUserIdFromToken(token ?? "");
            }
        }

    }

}

