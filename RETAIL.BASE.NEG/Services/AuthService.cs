using System;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly IRepositoryBase<User, BaseFilter, int> _userRepository;
        private readonly IUserService _userservice;
        private readonly ICryptographerB _cryptographer;
        private IConfiguration _config;

        public AuthService(ILogger<AuthService> logger,
        IRepositoryBase<User, BaseFilter, int> userRepository,
        IUserService userservice,
        ICryptographerB cryptographer,
        IConfiguration config)
        {
            _userRepository = userRepository;
            _userservice = userservice;
            _cryptographer = cryptographer;
            _config = config;
            _logger = logger;
        }

        #region Implementation of IAuthService
        public async Task<ResultModel<bool>> AuthenticateUserAsync(LoginModel login, int idUserAutenticado)
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
                    webResult.Message = "Autenticación realizada exitosamente con user predeterminado.";
                    webResult.Success = true;
                    return webResult;
                }

                User user = (await _userservice.GetByUsernameAsync(login.Username)).Data;
                if (user == null)
                {
                    webResult.Data = false;
                    webResult.Message = "User no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                isAuthenticated = _cryptographer.CompareHash(login.Password, user.Password);


                if (isAuthenticated)
                {
                    user.Attempts = 0;
                }

                else
                {
                    user.Attempts++;
                }

                if (user.Attempts >= 3)
                {
                    user.Enabled = false;
                }

                await _userservice.UpdateAsync(user, idUserAutenticado);

                if (!isAuthenticated)
                {
                    webResult.Data = false;
                    webResult.Message = "User o contraseña incorrectos.";
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
                _logger.LogError(ex, "Error al autenticar al user: {Username}", login.Username);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdatePasswordAsync(LoginModel login, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                User user = (await _userservice.GetByUsernameAsync(login.Username)).Data;

                if (user == null)
                {
                    webResult.Data = false;
                    webResult.Message = "User no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                user.Password = _cryptographer.CreateHash(input: login.Password);
                user.IdUserModification = idUserAutenticado;
                user.DateTimeModification = DateTime.UtcNow;

                webResult.Data = await _userRepository.UpdateAsync(user);
                webResult.Message = webResult.Data ? "Actualización de contraseña realizada exitosamente." : "Error al actualizar la contraseña.";
                webResult.Success = webResult.Data;
            }
            catch (Exception ex)
            {
                webResult.Message = "Error al realizar la actualización de contraseña.";
                webResult.Errors.Add(ex.Message);
                _logger.LogError(ex, "Error al actualizar la contraseña para el user: {Username}", login.Username);
            }
            return webResult;
        }
        #endregion

    }
}