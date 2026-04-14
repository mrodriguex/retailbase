using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HARD.CORE.DAT.Interfaces;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HARD.CORE.NEG.Services
{
    public class PerfilService : IPerfilService
    {
        private readonly IRepositoryBase<Perfil, BaseFilter, int> _perfilRepository;

        private readonly IRepositoryBase<Usuario, BaseFilter, int> _usuarioRepository;
        private readonly ILogger<PerfilService> _logger;

        public PerfilService(ILogger<PerfilService> logger,
        IRepositoryBase<Perfil, BaseFilter, int> perfilRepository,
        IRepositoryBase<Usuario, BaseFilter, int> usuarioRepository,
         IConfiguration config)
        {
            _perfilRepository = perfilRepository;
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        #region Implementation od IServiceBase

        public async Task<ResultModel<Perfil>> GetByIdAsync(int idPerfil)
        {
            var webResult = new ResultModel<Perfil>();
            try
            {
                webResult.Data = await _perfilRepository.GetByIdAsync(idPerfil);
                webResult.Message = "Información del perfil obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del perfil con ID: {IdPerfil}", idPerfil);
                webResult.Message = "Error al obtener la información del perfil.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Perfil>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Perfil>>();
            try
            {
                webResult.Data = await _perfilRepository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los perfiles");
                webResult.Message = "Error al obtener la información de los perfiles.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Perfil perfil, int idUsuarioAuenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                perfil.IdUsuarioCreacion = idUsuarioAuenticado;
                perfil.IdUsuarioModificacion = idUsuarioAuenticado;
                perfil.FechaCreacion = DateTime.UtcNow;
                perfil.FechaModificacion = DateTime.UtcNow;
                webResult.Data = await _perfilRepository.AddAsync(perfil);
                webResult.Message = "Perfil agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el perfil");
                webResult.Message = "Error al agregar el perfil.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Perfil perfil, int idUsuarioAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                perfil.IdUsuarioModificacion = idUsuarioAuenticado;
                perfil.FechaModificacion = DateTime.UtcNow;
                webResult.Data = await _perfilRepository.UpdateAsync(perfil);
                webResult.Message = "Perfil actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el perfil con ID: {IdPerfil}", perfil.Id);
                webResult.Message = "Error al actualizar el perfil.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int idPerfil, int idUsuarioAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                webResult.Data = await _perfilRepository.DeleteAsync(idPerfil);
                webResult.Message = "Perfil eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el perfil con ID: {IdPerfil}", idPerfil);
                webResult.Message = "Error al eliminar el perfil.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        #endregion

        #region Implementation of IPerfilService
        public async Task<ResultModel<IEnumerable<Perfil>>> GetUserProfilesAsync(int idUsuario)
        {
            var webResult = new ResultModel<IEnumerable<Perfil>>();
            try
            {
                List<Perfil> perfiles = new List<Perfil>();
                Usuario usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
                if (usuario != null)
                {
                    perfiles = usuario.Perfiles;
                    perfiles ??= new List<Perfil>();
                }

                webResult.Data = perfiles;
                webResult.Message = "Información de los perfiles del usuario obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los perfiles del usuario con ID: {IdUsuario}", idUsuario);
                webResult.Message = "Error al obtener la información de los perfiles del usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }


        public async Task<ResultModel<bool>> AssignProfileToUserAsync(int idUsuario, int idPerfil)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Usuario usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
                Perfil perfil = await _perfilRepository.GetByIdAsync(idPerfil);
                if (usuario == null || perfil == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Usuario o perfil no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                var existingPerfil = usuario.Perfiles.FirstOrDefault(p => p.Id == idPerfil);
                if (existingPerfil != null)
                {
                    webResult.Data = false;
                    webResult.Message = "El perfil ya está asignado al usuario.";
                    webResult.Success = false;
                    return webResult;
                }

                usuario.Perfiles.Add(perfil);
                bool updateResult = await _usuarioRepository.UpdateAsync(usuario);
                webResult.Data = updateResult;
                webResult.Message = updateResult ? "Perfil asignado exitosamente." : "Error al asignar el perfil.";
                webResult.Success = updateResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar el perfil con ID: {IdPerfil} al usuario con ID: {IdUsuario}", idPerfil, idUsuario);
                webResult.Message = "Error al asignar el perfil al usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> RemoveProfileFromUserAsync(int idUsuario, int idPerfil)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Usuario usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
                Perfil perfil = await _perfilRepository.GetByIdAsync(idPerfil);
                if (usuario == null || perfil == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Usuario o perfil no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                var existingPerfil = usuario.Perfiles.FirstOrDefault(p => p.Id == idPerfil);
                if (existingPerfil == null)
                {
                    webResult.Data = false;
                    webResult.Message = "El perfil no está asignado al usuario.";
                    webResult.Success = false;
                    return webResult;
                }

                usuario.Perfiles.Remove(existingPerfil);
                bool updateResult = await _usuarioRepository.UpdateAsync(usuario);
                webResult.Data = updateResult;
                webResult.Message = updateResult ? "Perfil removido exitosamente." : "Error al remover el perfil.";
                webResult.Success = updateResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover el perfil con ID: {IdPerfil} del usuario con ID: {IdUsuario}", idPerfil, idUsuario);
                webResult.Message = "Error al remover el perfil del usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        #endregion

    }
}