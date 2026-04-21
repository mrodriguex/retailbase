using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class RoleService : IRoleService
    {
        private readonly IRepositoryBase<Role, BaseFilter, int> _roleRepository;

        private readonly IRepositoryBase<User, BaseFilter, int> _userRepository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(ILogger<RoleService> logger,
        IRepositoryBase<Role, BaseFilter, int> roleRepository,
        IRepositoryBase<User, BaseFilter, int> userRepository,
         IConfiguration config)
        {
            _roleRepository = roleRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        #region Implementation od IServiceBase

        public async Task<ResultModel<Role>> GetByIdAsync(int idRole)
        {
            var webResult = new ResultModel<Role>();
            try
            {
                webResult.Data = await _roleRepository.GetByIdAsync(idRole);
                webResult.Message = "Información del role obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del role con ID: {IdRole}", idRole);
                webResult.Message = "Error al obtener la información del role.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Role>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Role>>();
            try
            {
                webResult.Data = await _roleRepository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los roles");
                webResult.Message = "Error al obtener la información de los roles.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Role role, int idUserAuenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                role.IdUserCreation = idUserAuenticado;
                role.IdUserModification = idUserAuenticado;
                role.DateTimeCreation = DateTime.UtcNow;
                role.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _roleRepository.AddAsync(role);
                webResult.Message = "Role agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el role");
                webResult.Message = "Error al agregar el role.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Role role, int idUserAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                role.IdUserModification = idUserAuenticado;
                role.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _roleRepository.UpdateAsync(role);
                webResult.Message = "Role actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el role con ID: {IdRole}", role.Id);
                webResult.Message = "Error al actualizar el role.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int idRole, int idUserAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                webResult.Data = await _roleRepository.DeleteAsync(idRole);
                webResult.Message = "Role eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el role con ID: {IdRole}", idRole);
                webResult.Message = "Error al eliminar el role.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        #endregion

        #region Implementation of IRoleService
        public async Task<ResultModel<IEnumerable<Role>>> GetUserProfilesAsync(int idUser)
        {
            var webResult = new ResultModel<IEnumerable<Role>>();
            try
            {
                List<Role> roles = new List<Role>();
                User user = await _userRepository.GetByIdAsync(idUser);
                if (user != null)
                {
                    roles = user.Roles;
                    roles ??= new List<Role>();
                }

                webResult.Data = roles;
                webResult.Message = "Información de los roles del user obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los roles del user con ID: {IdUser}", idUser);
                webResult.Message = "Error al obtener la información de los roles del user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }


        public async Task<ResultModel<bool>> AssignProfileToUserAsync(int idUser, int idRole)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                User user = await _userRepository.GetByIdAsync(idUser);
                Role role = await _roleRepository.GetByIdAsync(idRole);
                if (user == null || role == null)
                {
                    webResult.Data = false;
                    webResult.Message = "User o role no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                var existingRole = user.Roles.FirstOrDefault(p => p.Id == idRole);
                if (existingRole != null)
                {
                    webResult.Data = false;
                    webResult.Message = "El role ya está asignado al user.";
                    webResult.Success = false;
                    return webResult;
                }

                user.Roles.Add(role);
                bool updateResult = await _userRepository.UpdateAsync(user);
                webResult.Data = updateResult;
                webResult.Message = updateResult ? "Role asignado exitosamente." : "Error al asignar el role.";
                webResult.Success = updateResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar el role con ID: {IdRole} al user con ID: {IdUser}", idRole, idUser);
                webResult.Message = "Error al asignar el role al user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> RemoveProfileFromUserAsync(int idUser, int idRole)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                User user = await _userRepository.GetByIdAsync(idUser);
                Role role = await _roleRepository.GetByIdAsync(idRole);
                if (user == null || role == null)
                {
                    webResult.Data = false;
                    webResult.Message = "User o role no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                var existingRole = user.Roles.FirstOrDefault(p => p.Id == idRole);
                if (existingRole == null)
                {
                    webResult.Data = false;
                    webResult.Message = "El role no está asignado al user.";
                    webResult.Success = false;
                    return webResult;
                }

                user.Roles.Remove(existingRole);
                bool updateResult = await _userRepository.UpdateAsync(user);
                webResult.Data = updateResult;
                webResult.Message = updateResult ? "Role removido exitosamente." : "Error al remover el role.";
                webResult.Success = updateResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover el role con ID: {IdRole} del user con ID: {IdUser}", idRole, idUser);
                webResult.Message = "Error al remover el role del user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        #endregion

    }
}