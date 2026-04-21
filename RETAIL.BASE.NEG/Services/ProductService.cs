using System;
using System.Threading.Tasks;
using RETAIL.BASE.DAT.Interfaces;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.OBJ;
using RETAIL.BASE.OBJ.Models;
using Microsoft.Extensions.Logging;

namespace RETAIL.BASE.NEG.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepositoryBase<Product, BaseFilter, int> _repository;
        private readonly ILogger<ProductService> _logger;

        public ProductService(ILogger<ProductService> logger, IRepositoryBase<Product, BaseFilter, int> repository)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<ResultModel<Product>> GetByIdAsync(int id)
        {
            var webResult = new ResultModel<Product>();
            try
            {
                webResult.Data = await _repository.GetByIdAsync(id);
                webResult.Message = "Información del producto obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el producto con ID: {Id}", id);
                webResult.Message = "Error al obtener la información del producto.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<PagedResult<Product>>> GetAllAsync(BaseFilter filterClass)
        {
            var webResult = new ResultModel<PagedResult<Product>>();
            try
            {
                webResult.Data = await _repository.GetAllAsync(filterClass);
                webResult.Message = "Información obtenida exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los productos");
                webResult.Message = "Error al obtener la información de los productos.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<int>> AddAsync(Product entity, int idUserAutenticado)
        {
            var webResult = new ResultModel<int>();
            try
            {
                entity.IdUserCreation = idUserAutenticado;
                entity.IdUserModification = idUserAutenticado;
                entity.DateTimeCreation = DateTime.UtcNow;
                entity.DateTimeModification = DateTime.UtcNow;
                webResult.Data = await _repository.AddAsync(entity);
                webResult.Message = "Producto agregado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar el producto");
                webResult.Message = "Error al agregar el producto.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }

        public async Task<ResultModel<bool>> UpdateAsync(Product entity, int idUserAutenticado)
        {
            var webResult = new ResultModel<bool>();
            try
            {
                Product current = await _repository.GetByIdAsync(entity.Id);
                if (current == null)
                {
                    webResult.Data = false;
                    webResult.Message = "Producto no encontrado.";
                    webResult.Success = false;
                    return webResult;
                }

                entity.IdUserModification = idUserAutenticado;
                entity.DateTimeModification = DateTime.UtcNow;
                await _repository.UpdateAsync(entity);
                webResult.Data = true;
                webResult.Message = "Producto actualizado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el producto con ID: {Id}", entity.Id);
                webResult.Message = "Error al actualizar el producto.";
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
                webResult.Message = "Producto eliminado exitosamente.";
                webResult.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el producto con ID: {Id}", id);
                webResult.Message = "Error al eliminar el producto.";
                webResult.Errors.Add(ex.Message);
            }
            return webResult;
        }
    }
}
