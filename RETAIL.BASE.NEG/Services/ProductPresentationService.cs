using System;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class ProductPresentationService : IProductPresentationService
    {
        private readonly IRepositoryBase<ProductPresentation, BaseFilter, int> _repository;
        private readonly ILogger<ProductPresentationService> _logger;

        public ProductPresentationService(ILogger<ProductPresentationService> logger, IRepositoryBase<ProductPresentation, BaseFilter, int> repository)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultModel<ProductPresentation>> GetByIdAsync(int id)
        {
            var webResult = new ResultModel<ProductPresentation>();
            try
            {
                webResult.Data = await _repository.GetByIdAsync(id);
                webResult.Message = "Información de la presentación obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la presentación con ID: {Id}", id);
                webResult.Message = "Error al obtener la información de la presentación.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<ProductPresentation>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<ProductPresentation>>();
            try
            {
                webResult.Data = await _repository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las presentaciones");
                webResult.Message = "Error al obtener la información de las presentaciones.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(ProductPresentation entity, int idUserAutenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                entity.IdUserCreation = idUserAutenticado;
                entity.IdUserModification = idUserAutenticado;
                entity.DateTimeCreation = DateTime.UtcNow;
                entity.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _repository.AddAsync(entity);
                webResult.Message = "Presentación agregada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar la presentación");
                webResult.Message = "Error al agregar la presentación.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(ProductPresentation entity, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                ProductPresentation current = await _repository.GetByIdAsync(entity.Id);
                if (current == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Presentación no encontrada.";
                    webResult.Success = false;
                    return webResult;
                }

                entity.IdUserModification = idUserAutenticado;
                entity.DateTimeModification = DateTime.UtcNow;
                await _repository.UpdateAsync(entity);
                webResult.Data = true;
                webResult.Message = "Presentación actualizada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la presentación con ID: {Id}", entity.Id);
                webResult.Message = "Error al actualizar la presentación.";
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
                webResult.Message = "Presentación eliminada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la presentación con ID: {Id}", id);
                webResult.Message = "Error al eliminar la presentación.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
    }
}
