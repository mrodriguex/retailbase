using Asp.Versioning;
using HARD.CORE.API.Controllers.Base;
using HARD.CORE.API.Helpers;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;

using System.Security.Claims;

using System.Text;

namespace HARD.CORE.API.Controllers.V2
{
    [AllowAnonymous]
    [ApiController]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")] // Version in the URL path 
    public class AuthController : BaseController
    {
        private readonly IConfiguration _config;
        private readonly IUsuarioService _usuarioService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AuthController"/> class.
        /// </summary>
        /// <param name="config">
        /// The configuration settings for the application.
        /// </param>
        /// <param name="usuarioB">
        /// The user business logic layer.
        /// </param>
        public AuthController(IConfiguration config, IUsuarioService usuarioService)
        {
            _config = config;
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
            webResult.Success = false;
            webResult.Message = "Error de autenticación";

            try
            {
                int tokenDuration = 60; //Default value
                int.TryParse(_config["Jwt:Duration"], out tokenDuration);   //Try parse token duration from appsettings.json, otherwise keep default value
                var jwtPrivKey = _config["Jwt:Key"] ?? "";
                webResult.Data = GenerateToken(login.Username);
                webResult.Success = true;
                webResult.Message = "Inicio de sesión exitoso";
            }
            catch (Exception ex)
            {
                webResult.Errors.Add(ex.Message);
            }

            return Ok(webResult);
        }


        public static string GenerateToken(string username)
        {
            var secret = "Cryoinfra_SDL_3d80b5da-824b-4dde-b1db-3942c6d3d9fc";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
        new Claim(ClaimTypes.Name, username),
        new Claim("username", username)
    };

            var token = new JwtSecurityToken(
                issuer: "Cryoinfra",
                audience: "SDM",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
