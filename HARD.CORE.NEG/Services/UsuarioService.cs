using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HARD.CORE.DAT.Interfaces;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace HARD.CORE.NEG.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IRepositoryBase<Usuario, BaseFilter, int> _usuarioRepository;
        private readonly ILogger<UsuarioService> _logger;

        public UsuarioService(ILogger<UsuarioService> logger,
        IRepositoryBase<Usuario, BaseFilter, int> usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        #region Implementation of IServiceBase

        public async Task<ResultModel<Usuario>> GetByIdAsync(int idUsuario)
        {
            var webResult = new ResultModel<Usuario>();
            try
            {
                webResult.Data = await _usuarioRepository.GetByIdAsync(idUsuario);
                webResult.Message = "Información del usuario obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del usuario con ID: {IdUsuario}", idUsuario);
                webResult.Message = "Error al obtener la información del usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Usuario>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Usuario>>();
            try
            {
                webResult.Data = await _usuarioRepository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los usuarios");
                webResult.Message = "Error al obtener la información de los usuarios.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Usuario usuario, int idUsuarioAutenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                usuario.IdUsuarioCreacion = idUsuarioAutenticado;
                usuario.IdUsuarioModificacion = idUsuarioAutenticado;
                usuario.FechaCreacion = DateTime.UtcNow;
                usuario.FechaModificacion = DateTime.UtcNow;
                webResult.Data = await _usuarioRepository.AddAsync(usuario);
                webResult.Message = "Usuario agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el usuario");
                webResult.Message = "Error al agregar el usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Usuario usuario, int idUsuarioAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Usuario usuarioCurrent = await _usuarioRepository.GetByIdAsync(usuario.Id);
                if (usuarioCurrent == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Usuario no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                // Preserve independent fields when request payload omits or sends empty values.
                if (string.IsNullOrWhiteSpace(usuario.Nombre))
                {
                    usuario.Nombre = usuarioCurrent.Nombre;
                }

                if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
                {
                    usuario.NombreUsuario = usuarioCurrent.NombreUsuario;
                }

                usuario.IdUsuarioModificacion = idUsuarioAutenticado;
                usuario.FechaModificacion = DateTime.UtcNow;
                usuario.Contrasena = usuarioCurrent.Contrasena;
                await _usuarioRepository.UpdateAsync(usuario);
                webResult.Data = true;
                webResult.Message = "Usuario actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el usuario con ID: {IdUsuario}", usuario.Id);
                webResult.Message = "Error al actualizar el usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int idUsuario, int idUsuarioAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                await _usuarioRepository.DeleteAsync(idUsuario);
                webResult.Data = true;
                webResult.Message = "Usuario eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el usuario con ID: {IdUsuario}", idUsuario);
                webResult.Message = "Error al eliminar el usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        #endregion

        #region Implementation of IUsuarioService

        public async Task<ResultModel<bool>> ExistsAsync(int idUsuario)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Usuario usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
                webResult.Data = usuario != null;
                webResult.Message = "Verificación de existencia del usuario realizada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar la existencia del usuario con ID: {IdUsuario}", idUsuario);
                webResult.Message = "Error al verificar la existencia del usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<Usuario>> GetByUsernameAsync(string username)
        {
            var webResult = new ResultModel<Usuario>();
            BaseFilter filter = new BaseFilter { PageIndex = 1, PageSize = int.MaxValue, Nombre = username };

            IEnumerable<Usuario> usuarios = (await _usuarioRepository.GetAllAsync(filter)).Data;
            webResult.Data = usuarios.FirstOrDefault();
            webResult.Success = webResult.Data != null;
            webResult.Message = webResult.Success ? "Usuario encontrado." : "Usuario no encontrado.";
            return webResult;
        }

        public async Task<ResultModel<bool>> UnlockUserAsync(int idUsuario, int idUsuarioAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Usuario usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
                if (usuario == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Usuario no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                usuario.NumeroIntentos = 0;
                usuario.IsActive = true;
                usuario.Activo = true;
                usuario.Bloqueado = false;

                usuario.IdUsuarioModificacion = idUsuarioAutenticado;
                usuario.FechaModificacion = DateTime.UtcNow;

                webResult.Data = await _usuarioRepository.UpdateAsync(usuario);
                webResult.Message = webResult.Data ? "Usuario desbloqueado exitosamente." : "Error al desbloquear el usuario.";
                webResult.Success = webResult.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desbloquear el usuario con ID: {IdUsuario}", idUsuario);
                webResult.Message = "Error al desbloquear el usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        
        public async Task<ResultModel<bool>> LockUserAsync(int idUsuario, int idUsuarioAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Usuario usuario = await _usuarioRepository.GetByIdAsync(idUsuario);
                if (usuario == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Usuario no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                usuario.NumeroIntentos = 0;
                usuario.IsActive = false;
                usuario.Activo = false;
                usuario.Bloqueado = true;

                usuario.IdUsuarioModificacion = idUsuarioAutenticado;
                usuario.FechaModificacion = DateTime.UtcNow;

                webResult.Data = await _usuarioRepository.UpdateAsync(usuario);
                webResult.Message = webResult.Data ? "Usuario bloqueado exitosamente." : "Error al bloquear el usuario.";
                webResult.Success = webResult.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al bloquear el usuario con ID: {IdUsuario}", idUsuario);
                webResult.Message = "Error al bloquear el usuario.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        #endregion
    }
}