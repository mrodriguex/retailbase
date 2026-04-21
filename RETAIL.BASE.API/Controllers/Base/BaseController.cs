using Asp.Viewsioning;
using RETAIL.BASE.API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace RETAIL.BASE.API.Controllers.Base
{
    [Authorize]
    [ApiController]
    [ApiViewsion("1.0")]
    [Route("api/v{version:apiViewsion}/[controller]")] // Viewsion in the URL path

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

