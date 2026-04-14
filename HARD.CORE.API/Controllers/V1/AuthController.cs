using Asp.Versioning;
using HARD.CORE.API.Controllers.Base;
using HARD.CORE.API.Helpers;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HARD.CORE.API.Controllers.V1
{
    /// <summary>
    /// Controller for user authentication.
    /// </summary>
    /// <remarks>
    /// This controller handles user login and token generation.
    /// </remarks>  

    [AllowAnonymous]
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path    
    public class AuthController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _authService;
        private readonly IUsuarioService _usuarioService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="config">
        /// The configuration settings for the application.
        /// </param>
        /// <param name="usuarioService">
        /// The user service layer. This service is responsible for handling user-related operations, such as retrieving user information and validating credentials.
        /// </param>
        public AuthController(IConfiguration config, IAuthService authService, IUsuarioService usuarioService)
        {
            _config = config;

            _authService = authService;
            _usuarioService = usuarioService;
        }

        /// <summary>
        /// Handles user login requests.
        /// </summary>
        /// <param name="login">The login credentials provided by the user.</param>
        /// <returns>
        /// An <see cref="IActionResult"/> containing a <c>ResultModel&lt;string&gt;</c> object with the result of the authentication process.
        /// If authentication is successful, returns a JWT token; otherwise, returns error messages indicating the reason for failure.
        /// </returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel login)
        {
            var webResult = new ResultModel<string>();

            try
            {
                if (!(await _authService.AuthenticateUserAsync(login, IdUsuarioAutenticado)).Data)
                {
                    webResult.Errors.Add("Credenciales son incorrectas");
                }
                else
                {

                    Usuario usuario = login.Username == "administrador" ? new Usuario() { Id = 0 } : (await _usuarioService.GetByUsernameAsync(login.Username)).Data;

                    int tokenDuration = 60; //Default value
                    int.TryParse(_config["Jwt:Duration"], out tokenDuration);   //Try parse token duration from appsettings.json, otherwise keep default value
                    var jwtPrivKey = _config["Jwt:Key"] ?? "";
                    webResult.Data = JwtAuthenticateHelper.GenerateJwtToken(usuario.Id, tokenDuration, jwtPrivKey);
                    webResult.Success = true;
                    webResult.Message = "Inicio de sesión exitoso";
                }

            }
            catch (Exception ex)
            {
                webResult.Errors.Add(ex.Message);
            }

            return Ok(webResult);
        }

    }

}
