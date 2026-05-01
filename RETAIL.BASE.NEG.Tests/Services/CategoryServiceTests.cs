using System;
using System.Collections.Generic;
using RETAIL.BASE.NEG.Services;
using RETAIL.BASE.OBJ;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RETAIL.BASE.NEG.Tests.Services
{
    public class CategoryServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Category, BaseFilter, int>> _repositoryMock;
        private readonly Mock<ILogger<CategoryService>> _loggerMock;
        private readonly CategoryService _service;

        public CategoryServiceTests()
        {
            _repositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Category, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<CategoryService>>();
            _service = new CategoryService(_loggerMock.Object, _repositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenCategoryExists_ReturnsSuccess()
        {
            var category = CreateCategory(1);
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(category);

            var result = await _service.GetByIdAsync(1);

            Assert.True(result.Success);
            Assert.Equal(category, result.Data);
            Assert.Equal("Información de la categoría obtenida exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(1)).ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(1);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de la categoría.", result.Message);
            Assert.Contains("get error", result.Errors);
        }

        [Fact]
        public async Task GetAll_WhenCategoriesExist_ReturnsSuccess()
        {
            var categories = new List<Category> { CreateCategory(1), CreateCategory(2) };
            var filter = new BaseFilter { Enabled = true, PageIndex = 1, PageSize = 10 };

            _repositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 1 && f.PageSize == 10 && f.Enabled == true)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<Category>
                {
                    Data = categories,
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
            Assert.Equal("Error al obtener la información de las categorías.", result.Message);
            Assert.Contains("list error", result.Errors);
        }

        [Fact]
        public async Task Add_WhenCategoryIsValid_InjectsAuditFieldsAndReturnsId()
        {
            var category = CreateCategory();
            Category? captured = null;
            var before = DateTime.UtcNow;

            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Category>()))
                .Callback<Category>(c => captured = c)
                .ReturnsAsync(20);

            var result = await _service.AddAsync(category, 33);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(20, result.Data);
            Assert.Equal("Categoría agregada exitosamente.", result.Message);
            Assert.NotNull(captured);
            Assert.Equal(33, captured!.IdUserCreation);
            Assert.Equal(33, captured.IdUserModification);
            Assert.InRange(captured.DateTimeCreation, before, after);
            Assert.InRange(captured.DateTimeModification, before, after);
            _repositoryMock.Verify(x => x.AddAsync(It.IsAny<Category>()), Times.Once);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Category>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateCategory(), 33);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar la categoría.", result.Message);
            Assert.Contains("insert error", result.Errors);
        }

        [Fact]
        public async Task Update_WhenCategoryExists_InjectsAuditFieldsAndReturnsSuccess()
        {
            var category = CreateCategory(8);
            Category? captured = null;
            var before = DateTime.UtcNow;

            _repositoryMock.Setup(x => x.GetByIdAsync(8)).ReturnsAsync(category);
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Category>()))
                .Callback<Category>(c => captured = c)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(category, 60);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Categoría actualizada exitosamente.", result.Message);
            Assert.NotNull(captured);
            Assert.Equal(60, captured!.IdUserModification);
            Assert.InRange(captured.DateTimeModification, before, after);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenCategoryNotFound_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(8)).ReturnsAsync((Category)null!);

            var result = await _service.UpdateAsync(CreateCategory(8), 60);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Categoría no encontrada.", result.Message);
            _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Category>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.GetByIdAsync(8)).ReturnsAsync(CreateCategory(8));
            _repositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Category>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateCategory(8), 60);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar la categoría.", result.Message);
            Assert.Contains("update error", result.Errors);
        }

        [Fact]
        public async Task Delete_WhenCategoryExists_ReturnsSuccess()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(9)).ReturnsAsync(true);

            var result = await _service.DeleteAsync(9, 60);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Categoría eliminada exitosamente.", result.Message);
            _repositoryMock.Verify(x => x.DeleteAsync(9), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _repositoryMock.Setup(x => x.DeleteAsync(9)).ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(9, 60);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar la categoría.", result.Message);
            Assert.Contains("delete error", result.Errors);
        }

        private static Category CreateCategory(int id = 0) =>
            new Category { Id = id, Name = "Category Test" };
    }
}
