using System;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class BrandService : IBrandService
    {
        private readonly IRepositoryBase<Brand, BaseFilter, int> _repository;
        private readonly ILogger<BrandService> _logger;

        public BrandService(ILogger<BrandService> logger, IRepositoryBase<Brand, BaseFilter, int> repository)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultModel<Brand>> GetByIdAsync(int id)
        {
            var webResult = new ResultModel<Brand>();
            try
            {
                webResult.Data = await _repository.GetByIdAsync(id);
                webResult.Message = "Información de la marca obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la marca con ID: {Id}", id);
                webResult.Message = "Error al obtener la información de la marca.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Brand>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Brand>>();
            try
            {
                webResult.Data = await _repository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las marcas");
                webResult.Message = "Error al obtener la información de las marcas.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Brand entity, int idUserAutenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                entity.IdUserCreation = idUserAutenticado;
                entity.IdUserModification = idUserAutenticado;
                entity.DateTimeCreation = DateTime.UtcNow;
                entity.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _repository.AddAsync(entity);
                webResult.Message = "Marca agregada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar la marca");
                webResult.Message = "Error al agregar la marca.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Brand entity, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Brand current = await _repository.GetByIdAsync(entity.Id);
                if (current == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Marca no encontrada.";
                    webResult.Success = false;
                    return webResult;
                }

                entity.IdUserModification = idUserAutenticado;
                entity.DateTimeModification = DateTime.UtcNow;
                await _repository.UpdateAsync(entity);
                webResult.Data = true;
                webResult.Message = "Marca actualizada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la marca con ID: {Id}", entity.Id);
                webResult.Message = "Error al actualizar la marca.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> DeleteAsync(int id, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                await _repository.DeleteAsync(id);
                webResult.Data = true;
                webResult.Message = "Marca eliminada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la marca con ID: {Id}", id);
                webResult.Message = "Error al eliminar la marca.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
    }
}
