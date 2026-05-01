using System;
using System.Collections.Generic;
using RETAIL.BASE.NEG.Services;
using RETAIL.BASE.OBJ;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RETAIL.BASE.NEG.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Product, BaseFilter, int>> _repositoryMock;
        private readonly Mock<ILogger<ProductService>> _loggerMock;
        private readonly ProductService _service;

        public ProductServiceTests()
        {
            _repositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Product, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<ProductService>>();
            _service = new ProductService(_loggerMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenProductExists_ReturnsSuccess()
        {
            var product = CreateProduct(1);
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(product);

            var result = await _service.GetByIdAsync(1);

            Assert.True(result.Success);
            Assert.Equal(product, result.Data);
            Assert.Equal("Información del producto obtenida exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(1);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del producto.", result.Message);
            Assert.Contains("get error", result.Errors);
        }

        [Fact]
        public async Task GetAll_WhenProductsExist_ReturnsSuccess()
        {
            var products = new List<Product> { CreateProduct(1), CreateProduct(2) };
            var filter = new BaseFilter { Enabled = true, PageIndex = 1, PageSize = 10 };

            _repositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 1 && f.PageSize == 10 && f.Enabled == true)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<Product>
                {
                    Data = products,
                    TotalCount = 2,
                    PageIndex = 1,
                    PageSize = 10
                });

            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .ThrowsAsync(new Exception("list error"));

            var result = await _service.GetAllAsync(new BaseFilter { PageIndex = 1, PageSize = 10 });

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los productos.", result.Message);
            Assert.Contains("list error", result.Errors);
        }

        [Fact]
        public async Task Add_WhenProductIsValid_InjectsAuditFieldsAndReturnsId()
        {
            var product = CreateProduct();
            Product? captured = null;
            var before = DateTime.UtcNow;

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => captured = p)
                .ReturnsAsync(15);

            var result = await _service.AddAsync(product, 77);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(15, result.Data);
            Assert.Equal("Producto agregado exitosamente.", result.Message);
            Assert.NotNull(captured);
            Assert.Equal(77, captured!.IdUserCreation);
            Assert.Equal(77, captured.IdUserModification);
            Assert.InRange(captured.DateTimeCreation, before, after);
            Assert.InRange(captured.DateTimeModification, before, after);
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Product>()), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Product>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateProduct(), 77);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el producto.", result.Message);
            Assert.Contains("insert error", result.Errors);
        }

        [Fact]
        public async Task Update_WhenProductExists_InjectsAuditFieldsAndReturnsSuccess()
        {
            var product = CreateProduct(3);
            Product? captured = null;
            var before = DateTime.UtcNow;

            _repositoryMock.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(product);
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
                .Callback<Product>(p => captured = p)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(product, 77);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Producto actualizado exitosamente.", result.Message);
            Assert.NotNull(captured);
            Assert.Equal(77, captured!.IdUserModification);
            Assert.InRange(captured.DateTimeModification, before, after);
            _repositoryMock.Verify(x => x.GetByIdAsync(3), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenProductNotFound_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(3)).ReturnsAsync((Product)null!);

            var result = await _service.UpdateAsync(CreateProduct(3), 77);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Producto no encontrado.", result.Message);
            _repositoryMock.Verify(x => x.GetByIdAsync(3), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(3)).ReturnsAsync(CreateProduct(3));
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateProduct(3), 77);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el producto.", result.Message);
            Assert.Contains("update error", result.Errors);
        }

        [Fact]
        public async Task Delete_WhenProductExists_ReturnsSuccess()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(6)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(6, 77);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Producto eliminado exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.DeleteAsync(6), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(6)).ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(6, 77);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el producto.", result.Message);
            Assert.Contains("delete error", result.Errors);
        }

        private static Product CreateProduct(int id = 0) =>
            new Product { Id = id, Name = "Product Test", BrandId = 1, CategoryId = 1 };
    }
}
