using System;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IRepositoryBase<Category, BaseFilter, int> _repository;
        private readonly ILogger<CategoryService> _logger;

        public CategoryService(ILogger<CategoryService> logger, IRepositoryBase<Category, BaseFilter, int> repository)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultModel<Category>> GetByIdAsync(int id)
        {
            var webResult = new ResultModel<Category>();
            try
            {
                webResult.Data = await _repository.GetByIdAsync(id);
                webResult.Message = "Información de la categoría obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la categoría con ID: {Id}", id);
                webResult.Message = "Error al obtener la información de la categoría.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Category>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Category>>();
            try
            {
                webResult.Data = await _repository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las categorías");
                webResult.Message = "Error al obtener la información de las categorías.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Category entity, int idUserAutenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                entity.IdUserCreation = idUserAutenticado;
                entity.IdUserModification = idUserAutenticado;
                entity.DateTimeCreation = DateTime.UtcNow;
                entity.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _repository.AddAsync(entity);
                webResult.Message = "Categoría agregada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar la categoría");
                webResult.Message = "Error al agregar la categoría.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Category entity, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Category current = await _repository.GetByIdAsync(entity.Id);
                if (current == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Categoría no encontrada.";
                    webResult.Success = false;
                    return webResult;
                }

                entity.IdUserModification = idUserAutenticado;
                entity.DateTimeModification = DateTime.UtcNow;
                await _repository.UpdateAsync(entity);
                webResult.Data = true;
                webResult.Message = "Categoría actualizada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría con ID: {Id}", entity.Id);
                webResult.Message = "Error al actualizar la categoría.";
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
                webResult.Message = "Categoría eliminada exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría con ID: {Id}", id);
                webResult.Message = "Error al eliminar la categoría.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
    }
}
