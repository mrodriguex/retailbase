using System;
using System.Collections.Generic;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.NEG.Services;
using RETAIL.BASE.OBJ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RETAIL.BASE.NEG.Tests.Services
{
    public class MenuItemServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<MenuItem, BaseFilter, int>> _menuitemRepositoryMock;
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<User, BaseFilter, int>> _userRepositoryMock;
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Role, BaseFilter, int>> _roleRepositoryMock;
        private readonly Mock<ILogger<MenuItemService>> _loggerMock;
        private readonly MenuItemService _service;

        public MenuItemServiceTests()
        {
            _menuitemRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<MenuItem, BaseFilter, int>>();
            _userRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<User, BaseFilter, int>>();
            _roleRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Role, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<MenuItemService>>();
            _service = new MenuItemService(_loggerMock.Object, _menuitemRepositoryMock.Object, _userRepositoryMock.Object, _roleRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenMenuItemExists_ReturnsSuccess()
        {
            var menuitem = CreateMenuItem(2);
            _menuitemRepositoryMock
                .Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(menuitem);

            var result = await _service.GetByIdAsync(2);

            Assert.True(result.Success);
            Assert.Equal(menuitem, result.Data);
            Assert.Equal("Información del menuitem obtenida exitosamente.", result.Message);
            _menuitemRepositoryMock.Verify(x => x.GetByIdAsync(2), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _menuitemRepositoryMock
                .Setup(x => x.GetByIdAsync(2))
                .ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(2);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del menuitem.", result.Message);
            Assert.Contains("get error", result.Errors);
            _menuitemRepositoryMock.Verify(x => x.GetByIdAsync(2), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenMenuItemsExist_ReturnsSuccess()
        {
            var menuitems = new List<MenuItem> { CreateMenuItem(1), CreateMenuItem(2) };
            var filter = new BaseFilter { Enabled = true, PageIndex = 2, PageSize = 50 };

            _menuitemRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 2 &&
                    f.PageSize == 50 &&
                    f.Enabled == true)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<MenuItem> { Data = menuitems, TotalCount = 2, PageIndex = 2, PageSize = 50 });

            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _menuitemRepositoryMock.Verify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 2 &&
                f.PageSize == 50 &&
                f.Enabled == true)), Times.Once);
            _menuitemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MenuItem>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _menuitemRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .ThrowsAsync(new Exception("list error"));

            var filter = new BaseFilter { Enabled = false, PageIndex = 1, PageSize = 5 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los menuitemes.", result.Message);
            Assert.Contains("list error", result.Errors);
            _menuitemRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenMenuItemIsValid_InyectsAuditFieldsAndReturnsId()
        {
            var menuitem = CreateMenuItem();
            MenuItem? capturedMenuItem = null;
            var before = DateTime.UtcNow;

            _menuitemRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<MenuItem>()))
                .Callback<MenuItem>(value => capturedMenuItem = value)
                .ReturnsAsync(14);

            var result = await _service.AddAsync(menuitem, 61);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(14, result.Data);
            Assert.Equal("MenuItem agregado exitosamente.", result.Message);
            Assert.NotNull(capturedMenuItem);
            Assert.Equal(61, capturedMenuItem.IdUserCreation);
            Assert.Equal(61, capturedMenuItem.IdUserModification);
            Assert.InRange(capturedMenuItem.DateTimeCreation, before, after);
            Assert.InRange(capturedMenuItem.DateTimeModification, before, after);
            _menuitemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MenuItem>()), Times.Once);
            _menuitemRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<MenuItem>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _menuitemRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<MenuItem>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateMenuItem(), 61);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el menuitem.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _menuitemRepositoryMock.Verify(x => x.AddAsync(It.IsAny<MenuItem>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenMenuItemIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var menuitem = CreateMenuItem(19);
            MenuItem? capturedMenuItem = null;
            var before = DateTime.UtcNow;

            _menuitemRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<MenuItem>()))
                .Callback<MenuItem>(value => capturedMenuItem = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(menuitem, 72);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("MenuItem actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedMenuItem);
            Assert.Equal(72, capturedMenuItem.IdUserModification);
            Assert.InRange(capturedMenuItem.DateTimeModification, before, after);
            _menuitemRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<MenuItem>()), Times.Once);
            _menuitemRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _menuitemRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<MenuItem>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateMenuItem(19), 72);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el menuitem.", result.Message);
            Assert.Contains("update error", result.Errors);
            _menuitemRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<MenuItem>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesMenuItem_ReturnsSuccess()
        {
            _menuitemRepositoryMock
                .Setup(x => x.DeleteAsync(25))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(25, 72);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("MenuItem eliminado exitosamente.", result.Message);
            _menuitemRepositoryMock.Verify(x => x.DeleteAsync(25), Times.Once);
            _menuitemRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _menuitemRepositoryMock
                .Setup(x => x.DeleteAsync(25))
                .ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(25, 72);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el menuitem.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _menuitemRepositoryMock.Verify(x => x.DeleteAsync(25), Times.Once);
        }

        [Fact]
        public async Task GetMenuItemsByUser_WhenMenuItemsExist_ReturnsSuccess()
        {
            var menuitems = new List<MenuItem> { CreateMenuItem(31), CreateMenuItem(32) };
            var role = new Role { Id = 9, MenuItems = menuitems };
            var user = new User { Id = 8, Roles = new List<Role> { role } };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(8))
                .ReturnsAsync(user);

            var result = await _service.GetMenuItemsByUserAsync(8, 9);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.ToList().Count);
            Assert.Equal("Información de los menuitemes del user obtenida exitosamente.", result.Message);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(8), Times.Once);
        }

        [Fact]
        public async Task GetMenuItemsByUser_WhenBusinessThrows_ReturnsFailure()
        {
            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(8))
                .ThrowsAsync(new Exception("user menuitems error"));

            var result = await _service.GetMenuItemsByUserAsync(8, 9);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los menuitemes del user.", result.Message);
            Assert.Contains("user menuitems error", result.Errors);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(8), Times.Once);
        }

        [Fact]
        public async Task GetMenuItemsByProfile_WhenMenuItemsExist_ReturnsSuccess()
        {
            var menuitems = new List<MenuItem> { CreateMenuItem(41), CreateMenuItem(42) };
            var role = new Role { Id = 3, MenuItems = menuitems };

            _roleRepositoryMock
                .Setup(x => x.GetByIdAsync(3))
                .ReturnsAsync(role);

            var result = await _service.GetMenuItemsByProfileAsync(3);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.ToList().Count);
            Assert.Equal("Información de los menuitemes del role obtenida exitosamente.", result.Message);
            _roleRepositoryMock.Verify(x => x.GetByIdAsync(3), Times.Once);
        }

        [Fact]
        public async Task GetMenuItemsByProfile_WhenBusinessThrows_ReturnsFailure()
        {
            _roleRepositoryMock
                .Setup(x => x.GetByIdAsync(3))
                .Throws(new Exception("profile menuitems error"));

            var result = await _service.GetMenuItemsByProfileAsync(3);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los menuitemes del role.", result.Message);
            Assert.Contains("profile menuitems error", result.Errors);
            _roleRepositoryMock.Verify(x => x.GetByIdAsync(3), Times.Once);
        }

        private static MenuItem CreateMenuItem(int id = 1)
        {
            return new MenuItem
            {
                Id = id,
                Name = "MenuItem Demo",
                Path = "/demo",
                Image = "demo.png"
            };
        }
    }
}
