using System;
using System.Threading.Tasks;
using HARD.CORE.DAT.Interfaces;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HARD.CORE.NEG.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IRepositoryBase<Usuario, BaseFilter, int> _usuarioRepository;
        private readonly IUsuarioService _usuarioService;
        private readonly ICryptographerB _cryptographer;
        private IConfiguration _config;

        public AuthService(ILogger<AuthService> logger,
        IRepositoryBase<Usuario, BaseFilter, int> usuarioRepository,
        IUsuarioService usuarioService,
        ICryptographerB cryptographer,
        IConfiguration config)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _cryptographer = cryptographer;
            _config = config;
            _logger = logger;
        }

        #region Implementation of IAuthService
        public async Task<ResultModel<bool>> AuthenticateUserAsync(LoginModel login, int idUsuarioAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                bool isAuthenticated = false;
                string defaultUser = _config["DefaultUser"];
                string defaultPassword = _config["DefaultPassword"];
                if (login.Username == defaultUser && login.Password == defaultPassword)
                {
                    webResult.Data = true;
                    webResult.Message = "Autenticación realizada exitosamente con usuario predeterminado.";
                    webResult.Success = true;
                    return webResult;
                }

                Usuario usuario = (await _usuarioService.GetByUsernameAsync(login.Username)).Data;
                if (usuario == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Usuario no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                isAuthenticated = _cryptographer.CompareHash(login.Password, usuario.Contrasena);


                if (isAuthenticated)
                {
                    usuario.NumeroIntentos = 0;
                }

                else
                {
                    usuario.NumeroIntentos++;
                }

                if (usuario.NumeroIntentos >= 3)
                {
                    usuario.Bloqueado = true;
                }

                await _usuarioService.UpdateAsync(usuario, idUsuarioAutenticado);

                if (!isAuthenticated)
                {
                    webResult.Data = false;
                    webResult.Message = "Usuario o contraseña incorrectos.";
                    webResult.Success = false;
                    return webResult;
                }

                webResult.Data = true;
                webResult.Message = "Autenticación realizada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                webResult.Message = "Error al realizar la autenticación.";
                webResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error al autenticar al usuario: {Username}", login.Username);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdatePasswordAsync(LoginModel login, int idUsuarioAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Usuario usuario = (await _usuarioService.GetByUsernameAsync(login.Username)).Data;

                if (usuario == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Usuario no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                usuario.Contrasena = _cryptographer.CreateHash(input: login.Password);
                usuario.CambioContrasena = false;
                usuario.IdUsuarioModificacion = idUsuarioAutenticado;
                usuario.FechaModificacion = DateTime.UtcNow;

                webResult.Data = await _usuarioRepository.UpdateAsync(usuario);
                webResult.Message = webResult.Data ? "Actualización de contraseña realizada exitosamente." : "Error al actualizar la contraseña.";
                webResult.Success = webResult.Data;
            }
            catch (Exception ex)
            {
                webResult.Message = "Error al realizar la actualización de contraseña.";
                webResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error al actualizar la contraseña para el usuario: {Username}", login.Username);
            }
            return webResult;
        }
        #endregion

    }
}