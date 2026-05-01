using System;
using System.Collections.Generic;
using RETAIL.BASE.NEG.Services;
using RETAIL.BASE.OBJ;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RETAIL.BASE.NEG.Tests.Services
{
    public class ProductPresentationServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<ProductPresentation, BaseFilter, int>> _repositoryMock;
        private readonly Mock<ILogger<ProductPresentationService>> _loggerMock;
        private readonly ProductPresentationService _service;

        public ProductPresentationServiceTests()
        {
            _repositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<ProductPresentation, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<ProductPresentationService>>();
            _service = new ProductPresentationService(_loggerMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenPresentationExists_ReturnsSuccess()
        {
            var presentation = CreatePresentation(1);
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(presentation);

            var result = await _service.GetByIdAsync(1);

            Assert.True(result.Success);
            Assert.Equal(presentation, result.Data);
            Assert.Equal("Información de la presentación obtenida exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(1);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de la presentación.", result.Message);
            Assert.Contains("get error", result.Errors);
        }

        [Fact]
        public async Task GetAll_WhenPresentationsExist_ReturnsSuccess()
        {
            var presentations = new List<ProductPresentation> { CreatePresentation(1), CreatePresentation(2) };
            var filter = new BaseFilter { Enabled = true, IdMaster = 5, PageIndex = 1, PageSize = 10 };

            _repositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 1 && f.PageSize == 10 && f.Enabled == true && f.IdMaster == 5)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<ProductPresentation>
                {
                    Data = presentations,
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
            Assert.Equal("Error al obtener la información de las presentaciones.", result.Message);
            Assert.Contains("list error", result.Errors);
        }

        [Fact]
        public async Task Add_WhenPresentationIsValid_InjectsAuditFieldsAndReturnsId()
        {
            var presentation = CreatePresentation();
            ProductPresentation? captured = null;
            var before = DateTime.UtcNow;

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<ProductPresentation>()))
                .Callback<ProductPresentation>(p => captured = p)
                .ReturnsAsync(25);

            var result = await _service.AddAsync(presentation, 88);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(25, result.Data);
            Assert.Equal("Presentación agregada exitosamente.", result.Message);
            Assert.NotNull(captured);
            Assert.Equal(88, captured!.IdUserCreation);
            Assert.Equal(88, captured.IdUserModification);
            Assert.InRange(captured.DateTimeCreation, before, after);
            Assert.InRange(captured.DateTimeModification, before, after);
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProductPresentation>()), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProductPresentation>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<ProductPresentation>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreatePresentation(), 88);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar la presentación.", result.Message);
            Assert.Contains("insert error", result.Errors);
        }

        [Fact]
        public async Task Update_WhenPresentationExists_InjectsAuditFieldsAndReturnsSuccess()
        {
            var presentation = CreatePresentation(4);
            ProductPresentation? captured = null;
            var before = DateTime.UtcNow;

            _repositoryMock.Setup(x => x.GetByIdAsync(4)).ReturnsAsync(presentation);
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<ProductPresentation>()))
                .Callback<ProductPresentation>(p => captured = p)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(presentation, 88);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Presentación actualizada exitosamente.", result.Message);
            Assert.NotNull(captured);
            Assert.Equal(88, captured!.IdUserModification);
            Assert.InRange(captured.DateTimeModification, before, after);
            _repositoryMock.Verify(x => x.GetByIdAsync(4), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProductPresentation>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenPresentationNotFound_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(4)).ReturnsAsync((ProductPresentation)null!);

            var result = await _service.UpdateAsync(CreatePresentation(4), 88);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Presentación no encontrada.", result.Message);
            _repositoryMock.Verify(x => x.GetByIdAsync(4), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<ProductPresentation>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(4)).ReturnsAsync(CreatePresentation(4));
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<ProductPresentation>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreatePresentation(4), 88);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar la presentación.", result.Message);
            Assert.Contains("update error", result.Errors);
        }

        [Fact]
        public async Task Delete_WhenPresentationExists_ReturnsSuccess()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(12)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(12, 88);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Presentación eliminada exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.DeleteAsync(12), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(12)).ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(12, 88);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar la presentación.", result.Message);
            Assert.Contains("delete error", result.Errors);
        }

        private static ProductPresentation CreatePresentation(int id = 0) =>
            new ProductPresentation
            {
                Id = id,
                Name = "Presentation Test",
                ProductId = 1,
                Barcode = "123456789",
                SizeLabel = "355ml",
                Unit = "ml",
                Presentation = "Lata"
            };
    }
}
