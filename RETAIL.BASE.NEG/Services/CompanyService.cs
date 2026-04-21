using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IRepositoryBase<Company, BaseFilter, int> _companyRepository;
        private readonly IRepositoryBase<User, BaseFilter, int> _userDA;
        private readonly ILogger<CompanyService> _logger;

        public CompanyService(ILogger<CompanyService> logger,
        IRepositoryBase<Company, BaseFilter, int> companyRepository,
        IRepositoryBase<User, BaseFilter, int> userDA)
        {
            _companyRepository = companyRepository;
            _userDA = userDA;
            _logger = logger;
        }

        #region Implementation of IServiceBase
        public async Task<ResultModel<Company>> GetByIdAsync(int idCompany)
        {
            var webResult = new ResultModel<Company>();
            try
            {
                webResult.Data = await _companyRepository.GetByIdAsync(idCompany);
                webResult.Message = "Información del company obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información del company con ID: {IdCompany}", idCompany);
                webResult.Message = "Error al obtener la información del company.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Company>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Company>>();
            try
            {
                webResult.Data = await _companyRepository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los companyes");
                webResult.Message = "Error al obtener la información de los companyes.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Company company, int idUserAuenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                company.IdUserCreation = idUserAuenticado;
                company.IdUserModification = idUserAuenticado;
                company.DateTimeCreation = DateTime.UtcNow;
                company.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _companyRepository.AddAsync(company);
                webResult.Message = "Company agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el company");
                webResult.Message = "Error al agregar el company.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Company company, int idUserAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                company.IdUserModification = idUserAuenticado;
                company.DateTimeModification = DateTime.UtcNow;
                await _companyRepository.UpdateAsync(company);
                webResult.Data = true;
                webResult.Message = "Company actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el company con ID: {IdCompany}", company.Id);
                webResult.Message = "Error al actualizar el company.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int idCompany, int idUserAuenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                webResult.Data = await _companyRepository.DeleteAsync(idCompany);
                webResult.Message = "Company eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el company con ID: {IdCompany}", idCompany);
                webResult.Message = "Error al eliminar el company.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
        #endregion

        #region Implementation of ICompanyService
        public async Task<ResultModel<IEnumerable<Company>>> GetCompaniesByUserAsync(int idUser, int pageIndex = 1, int pageSize = int.MaxValue)
        {
            var webResult = new ResultModel<IEnumerable<Company>>();
            try
            {
                User user = await _userDA.GetByIdAsync(idUser);
                List<Company> companys = new List<Company>();
                if (user != null)
                {
                    user.Companys ??= new List<Company>();
                    companys = user.Companys;
                }
                webResult.Data = companys;
                webResult.Message = "Información de los companys del user obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la información de los companys del user con ID: {IdUser}", idUser);
                webResult.Message = "Error al obtener la información de los companys del user.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        #endregion

    }
}