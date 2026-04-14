using System;
using System.Threading.Tasks;
using HARD.CORE.DAT.Interfaces;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.OBJ;
using HARD.CORE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace HARD.CORE.NEG.Services
{
    public class ClienteService : IClienteService
    {
        private readonly ILogger<ClienteService> _logger;
        private readonly IRepositoryBase<Cliente, BaseFilter, int> _clienteRepository;

        public ClienteService(ILogger<ClienteService> logger, IRepositoryBase<Cliente, BaseFilter, int> clienteRepository)
        {
            _clienteRepository = clienteRepository;
            _logger = logger;
        }

        #region Implementation of IServiceBase
        public async Task<ResultModel<Cliente>> GetByIdAsync(int idCliente)
        {
            var webResult = new ResultModel<Cliente>();
            try
            {
                webResult.Data = await _clienteRepository.GetByIdAsync(idCliente);
                webResult.Message = "Información del cliente obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del cliente con ID: {IdCliente}", idCliente);
                webResult.Message = "Error al obtener la información del cliente.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Cliente>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Cliente>>();
            try
            {
                webResult.Data = await _clienteRepository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los clientes");
                webResult.Message = "Error al obtener la información de los clientes.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Cliente cliente, int idUsuarioAuenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                cliente.IdUsuarioCreacion = idUsuarioAuenticado;
                cliente.IdUsuarioModificacion = idUsuarioAuenticado;
                cliente.FechaCreacion = DateTime.UtcNow;
                cliente.FechaModificacion = DateTime.UtcNow;
                webResult.Data = await _clienteRepository.AddAsync(cliente);
                webResult.Message = "Cliente agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el cliente");
                webResult.Message = "Error al agregar el cliente.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Cliente cliente, int idUsuarioAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                cliente.IdUsuarioModificacion = idUsuarioAuenticado;
                cliente.FechaModificacion = DateTime.UtcNow;
                await _clienteRepository.UpdateAsync(cliente);
                webResult.Data = true;
                webResult.Message = "Cliente actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el cliente con ID: {IdCliente}", cliente.Id);
                webResult.Message = "Error al actualizar el cliente.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int idCliente, int idUsuarioAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                webResult.Data = await _clienteRepository.DeleteAsync(idCliente);
                webResult.Message = "Cliente eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el cliente con ID: {IdCliente}", idCliente);
                webResult.Message = "Error al eliminar el cliente.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        #endregion

    }
}