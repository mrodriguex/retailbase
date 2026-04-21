using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class UserService : IUserService
    {
        private readonly IRepositoryBase<User, BaseFilter, int> _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(ILogger<UserService> logger,
        IRepositoryBase<User, BaseFilter, int> userRepository)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        #region Implementation of IServiceBase

        public async Task<ResultModel<User>> GetByIdAsync(int idUser)
        {
            var webResult = new ResultModel<User>();
            try
            {
                webResult.Data = await _userRepository.GetByIdAsync(idUser);
                webResult.Message = "Información del user obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del user con ID: {IdUser}", idUser);
                webResult.Message = "Error al obtener la información del user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<User>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<User>>();
            try
            {
                webResult.Data = await _userRepository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los users");
                webResult.Message = "Error al obtener la información de los users.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(User user, int idUserAutenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                user.IdUserCreation = idUserAutenticado;
                user.IdUserModification = idUserAutenticado;
                user.DateTimeCreation = DateTime.UtcNow;
                user.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _userRepository.AddAsync(user);
                webResult.Message = "User agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el user");
                webResult.Message = "Error al agregar el user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(User user, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                User userCurrent = await _userRepository.GetByIdAsync(user.Id);
                if (userCurrent == null)
                {
                    webResult.Data = false;
                    webResult.Message = "User no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                // Preserve independent fields when request payload omits or sends empty values.
                if (string.IsNullOrWhiteSpace(user.Name))
                {
                    user.Name = userCurrent.Name;
                }

                if (string.IsNullOrWhiteSpace(user.FirstName))
                {
                    user.FirstName = userCurrent.FirstName;
                }

                user.IdUserModification = idUserAutenticado;
                user.DateTimeModification = DateTime.UtcNow;
                user.Password = userCurrent.Password;
                await _userRepository.UpdateAsync(user);
                webResult.Data = true;
                webResult.Message = "User actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el user con ID: {IdUser}", user.Id);
                webResult.Message = "Error al actualizar el user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int idUser, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                await _userRepository.DeleteAsync(idUser);
                webResult.Data = true;
                webResult.Message = "User eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el user con ID: {IdUser}", idUser);
                webResult.Message = "Error al eliminar el user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        #endregion

        #region Implementation of IUserService

        public async Task<ResultModel<bool>> ExistsAsync(int idUser)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                User user = await _userRepository.GetByIdAsync(idUser);
                webResult.Data = user != null;
                webResult.Message = "Verificación de existencia del user realizada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia del user con ID: {IdUser}", idUser);
                webResult.Message = "Error al verificar la existencia del user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<User>> GetByUsernameAsync(string username)
        {
            var webResult = new ResultModel<User>();
            BaseFilter filter = new BaseFilter { PageIndex = 1, PageSize = int.MaxValue, Name = username };

            IEnumerable<User> users = (await _userRepository.GetAllAsync(filter)).Data;
            webResult.Data = users.FirstOrDefault();
            webResult.Success = webResult.Data != null;
            webResult.Message = webResult.Success ? "User encontrado." : "User no encontrado.";
            return webResult;
        }

        public async Task<ResultModel<bool>> UnlockUserAsync(int idUser, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                User user = await _userRepository.GetByIdAsync(idUser);
                if (user == null)
                {
                    webResult.Data = false;
                    webResult.Message = "User no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                user.Attempts = 0;
                user.Enabled = true;

                user.IdUserModification = idUserAutenticado;
                user.DateTimeModification = DateTime.UtcNow;

                webResult.Data = await _userRepository.UpdateAsync(user);
                webResult.Message = webResult.Data ? "User desbloqueado exitosamente." : "Error al desbloquear el user.";
                webResult.Success = webResult.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desbloquear el user con ID: {IdUser}", idUser);
                webResult.Message = "Error al desbloquear el user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        
        public async Task<ResultModel<bool>> LockUserAsync(int idUser, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                User user = await _userRepository.GetByIdAsync(idUser);
                if (user == null)
                {
                    webResult.Data = false;
                    webResult.Message = "User no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                user.Attempts = 0;
                user.Enabled = false;

                user.IdUserModification = idUserAutenticado;
                user.DateTimeModification = DateTime.UtcNow;

                webResult.Data = await _userRepository.UpdateAsync(user);
                webResult.Message = webResult.Data ? "User bloqueado exitosamente." : "Error al bloquear el user.";
                webResult.Success = webResult.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al bloquear el user con ID: {IdUser}", idUser);
                webResult.Message = "Error al bloquear el user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        #endregion
    }
}