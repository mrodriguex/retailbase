using System;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly ILogger<CustomerService> _logger;
        private readonly IRepositoryBase<Customer, BaseFilter, int> _customerRepository;

        public CustomerService(ILogger<CustomerService> logger, IRepositoryBase<Customer, BaseFilter, int> customerRepository)
        {
            _customerRepository = customerRepository;
            _logger = logger;
        }

        #region Implementation of IServiceBase
        public async Task<ResultModel<Customer>> GetByIdAsync(int idCustomer)
        {
            var webResult = new ResultModel<Customer>();
            try
            {
                webResult.Data = await _customerRepository.GetByIdAsync(idCustomer);
                webResult.Message = "Información del customer obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del customer con ID: {IdCustomer}", idCustomer);
                webResult.Message = "Error al obtener la información del customer.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Customer>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Customer>>();
            try
            {
                webResult.Data = await _customerRepository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los customers");
                webResult.Message = "Error al obtener la información de los customers.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Customer customer, int idUserAuenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                customer.IdUserCreation = idUserAuenticado;
                customer.IdUserModification = idUserAuenticado;
                customer.DateTimeCreation = DateTime.UtcNow;
                customer.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _customerRepository.AddAsync(customer);
                webResult.Message = "Customer agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el customer");
                webResult.Message = "Error al agregar el customer.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Customer customer, int idUserAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                customer.IdUserModification = idUserAuenticado;
                customer.DateTimeModification = DateTime.UtcNow;
                await _customerRepository.UpdateAsync(customer);
                webResult.Data = true;
                webResult.Message = "Customer actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el customer con ID: {IdCustomer}", customer.Id);
                webResult.Message = "Error al actualizar el customer.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int idCustomer, int idUserAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                webResult.Data = await _customerRepository.DeleteAsync(idCustomer);
                webResult.Message = "Customer eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el customer con ID: {IdCustomer}", idCustomer);
                webResult.Message = "Error al eliminar el customer.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        #endregion

    }
}