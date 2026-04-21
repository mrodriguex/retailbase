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
    public class MenuItemService : IMenuItemService
    {
        private readonly IRepositoryBase<MenuItem, BaseFilter, int> _menuitemRepository;
        private readonly IRepositoryBase<User, BaseFilter, int> _userRepository;
        private readonly IRepositoryBase<Role, BaseFilter, int> _roleRepository;
        private readonly ILogger<MenuItemService> _logger;

        public MenuItemService(ILogger<MenuItemService> logger,
        IRepositoryBase<MenuItem, BaseFilter, int> menuitemRepository,
        IRepositoryBase<User, BaseFilter, int> userRepository,
        IRepositoryBase<Role, BaseFilter, int> roleRepository)
        {
            _menuitemRepository = menuitemRepository;
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _logger = logger;
        }

        #region Implementation of IServiceBase
        public async Task<ResultModel<MenuItem>> GetByIdAsync(int idMenuItem)
        {
            var webResult = new ResultModel<MenuItem>();
            try
            {
                webResult.Data = await _menuitemRepository.GetByIdAsync(idMenuItem);
                webResult.Message = "Información del menuitem obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del menuitem con ID: {IdMenuItem}", idMenuItem);
                webResult.Message = "Error al obtener la información del menuitem.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<MenuItem>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<MenuItem>>();
            try
            {
                webResult.Data = (await _menuitemRepository.GetAllAsync(filterClass));
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los menuitemes");
                webResult.Message = "Error al obtener la información de los menuitemes.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(MenuItem menuitem, int idUserAuenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                menuitem.IdUserCreation = idUserAuenticado;
                menuitem.IdUserModification = idUserAuenticado;
                menuitem.DateTimeCreation = DateTime.UtcNow;
                menuitem.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _menuitemRepository.AddAsync(menuitem);
                webResult.Message = "MenuItem agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el menuitem");
                webResult.Message = "Error al agregar el menuitem.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(MenuItem menuitem, int idUserAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                menuitem.IdUserModification = idUserAuenticado;
                menuitem.DateTimeModification = DateTime.UtcNow;

                webResult.Data = await _menuitemRepository.UpdateAsync(menuitem);
                webResult.Message = "MenuItem actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el menuitem con ID: {IdMenuItem}", menuitem.Id);
                webResult.Message = "Error al actualizar el menuitem.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int idMenuItem, int idUserAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                webResult.Data = await _menuitemRepository.DeleteAsync(idMenuItem);
                webResult.Message = "MenuItem eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el menuitem con ID: {IdMenuItem}", idMenuItem);
                webResult.Message = "Error al eliminar el menuitem.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        #endregion

        #region Implementation of IMenuItemService

        public async Task<ResultModel<IEnumerable<MenuItem>>> GetMenuItemsByUserAsync(int idUser, int idRole)
        {
            var webResult = new ResultModel<IEnumerable<MenuItem>>();
            try
            {
                User user = await _userRepository.GetByIdAsync(idUser);
                List<MenuItem> menuitems = user.Roles.Where(p => p.Id == idRole)
                    .SelectMany(p => p.MenuItems)
                    .ToList();
                webResult.Data = menuitems ?? new List<MenuItem>();
                webResult.Message = "Información de los menuitemes del user obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los menuitemes del user con ID: {IdUser}", idUser);
                webResult.Message = "Error al obtener la información de los menuitemes del user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<IEnumerable<MenuItem>>> GetMenuItemsByProfileAsync(int idRole)
        {
            var webResult = new ResultModel<IEnumerable<MenuItem>>();
            try
            {
                Role role = await _roleRepository.GetByIdAsync(idRole);
                List<MenuItem> menuitems = role.MenuItems.ToList();
                webResult.Data = menuitems ?? new List<MenuItem>();
                webResult.Message = "Información de los menuitemes del role obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los menuitemes del role con ID: {IdRole}", idRole);
                webResult.Message = "Error al obtener la información de los menuitemes del role.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        #endregion

    }
}