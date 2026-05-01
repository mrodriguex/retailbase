using System;
using System.Collections.Generic;
using RETAIL.BASE.NEG.Services;
using RETAIL.BASE.OBJ;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RETAIL.BASE.NEG.Tests.Services
{
    public class BrandServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Brand, BaseFilter, int>> _repositoryMock;
        private readonly Mock<ILogger<BrandService>> _loggerMock;
        private readonly BrandService _service;

        public BrandServiceTests()
        {
            _repositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Brand, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<BrandService>>();
            _service = new BrandService(_loggerMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenBrandExists_ReturnsSuccess()
        {
            var brand = CreateBrand(1);
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(brand);

            var result = await _service.GetByIdAsync(1);

            Assert.True(result.Success);
            Assert.Equal(brand, result.Data);
            Assert.Equal("Información de la marca obtenida exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(1);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de la marca.", result.Message);
            Assert.Contains("get error", result.Errors);
            _repositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenBrandsExist_ReturnsSuccess()
        {
            var brands = new List<Brand> { CreateBrand(1), CreateBrand(2) };
            var filter = new BaseFilter { Enabled = true, PageIndex = 2, PageSize = 15 };

            _repositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 2 && f.PageSize == 15 && f.Enabled == true)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<Brand>
                {
                    Data = brands,
                    TotalCount = 2,
                    PageIndex = 2,
                    PageSize = 15
                });

            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 2 && f.PageSize == 15 && f.Enabled == true)), Times.Once);
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
            Assert.Equal("Error al obtener la información de las marcas.", result.Message);
            Assert.Contains("list error", result.Errors);
        }

        [Fact]
        public async Task Add_WhenBrandIsValid_InjectsAuditFieldsAndReturnsId()
        {
            var brand = CreateBrand();
            Brand? captured = null;
            var before = DateTime.UtcNow;

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Brand>()))
                .Callback<Brand>(b => captured = b)
                .ReturnsAsync(10);

            var result = await _service.AddAsync(brand, 42);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(10, result.Data);
            Assert.Equal("Marca agregada exitosamente.", result.Message);
            Assert.NotNull(captured);
            Assert.Equal(42, captured!.IdUserCreation);
            Assert.Equal(42, captured.IdUserModification);
            Assert.InRange(captured.DateTimeCreation, before, after);
            Assert.InRange(captured.DateTimeModification, before, after);
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Brand>()), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Brand>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Brand>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateBrand(), 42);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar la marca.", result.Message);
            Assert.Contains("insert error", result.Errors);
        }

        [Fact]
        public async Task Update_WhenBrandExists_InjectsAuditFieldsAndReturnsSuccess()
        {
            var brand = CreateBrand(5);
            Brand? captured = null;
            var before = DateTime.UtcNow;

            _repositoryMock.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(brand);
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Brand>()))
                .Callback<Brand>(b => captured = b)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(brand, 55);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Marca actualizada exitosamente.", result.Message);
            Assert.NotNull(captured);
            Assert.Equal(55, captured!.IdUserModification);
            Assert.InRange(captured.DateTimeModification, before, after);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Brand>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenBrandNotFound_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(5)).ReturnsAsync((Brand)null!);

            var result = await _service.UpdateAsync(CreateBrand(5), 55);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Marca no encontrada.", result.Message);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Brand>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(5)).ReturnsAsync(CreateBrand(5));
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Brand>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateBrand(5), 55);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar la marca.", result.Message);
            Assert.Contains("update error", result.Errors);
        }

        [Fact]
        public async Task Delete_WhenBrandExists_ReturnsSuccess()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(7)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(7, 55);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Marca eliminada exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.DeleteAsync(7), Times.Once);
            _repositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(7)).ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(7, 55);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar la marca.", result.Message);
            Assert.Contains("delete error", result.Errors);
        }

        private static Brand CreateBrand(int id = 0) =>
            new Brand { Id = id, Name = "Brand Test" };
    }
}
