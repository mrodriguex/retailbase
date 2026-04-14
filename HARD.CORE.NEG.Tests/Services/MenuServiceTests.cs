using System;
using System.Collections.Generic;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.NEG.Services;
using HARD.CORE.OBJ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HARD.CORE.NEG.Tests.Services
{
    public class MenuServiceTests
    {
        private readonly Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Menu, BaseFilter, int>> _menuRepositoryMock;
        private readonly Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Usuario, BaseFilter, int>> _usuarioRepositoryMock;
        private readonly Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Perfil, BaseFilter, int>> _perfilRepositoryMock;
        private readonly Mock<ILogger<MenuService>> _loggerMock;
        private readonly MenuService _service;

        public MenuServiceTests()
        {
            _menuRepositoryMock = new Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Menu, BaseFilter, int>>();
            _usuarioRepositoryMock = new Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Usuario, BaseFilter, int>>();
            _perfilRepositoryMock = new Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Perfil, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<MenuService>>();
            _service = new MenuService(_loggerMock.Object, _menuRepositoryMock.Object, _usuarioRepositoryMock.Object, _perfilRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenMenuExists_ReturnsSuccess()
        {
            var menu = CreateMenu(2);
            _menuRepositoryMock
                .Setup(x => x.GetByIdAsync(2))
                .ReturnsAsync(menu);

            var result = await _service.GetByIdAsync(2);

            Assert.True(result.Success);
            Assert.Equal(menu, result.Data);
            Assert.Equal("Información del menu obtenida exitosamente.", result.Message);
            _menuRepositoryMock.Verify(x => x.GetByIdAsync(2), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _menuRepositoryMock
                .Setup(x => x.GetByIdAsync(2))
                .ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(2);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del menu.", result.Message);
            Assert.Contains("get error", result.Errors);
            _menuRepositoryMock.Verify(x => x.GetByIdAsync(2), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenMenusExist_ReturnsSuccess()
        {
            var menus = new List<Menu> { CreateMenu(1), CreateMenu(2) };
            var filter = new BaseFilter { Activo = true, PageIndex = 2, PageSize = 50 };

            _menuRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 2 &&
                    f.PageSize == 50 &&
                    f.Activo == true)))
                .ReturnsAsync(new HARD.CORE.OBJ.Models.PagedResult<Menu> { Data = menus, TotalCount = 2, PageIndex = 2, PageSize = 50 });

            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _menuRepositoryMock.Verify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 2 &&
                f.PageSize == 50 &&
                f.Activo == true)), Times.Once);
            _menuRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Menu>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _menuRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .ThrowsAsync(new Exception("list error"));

            var filter = new BaseFilter { Activo = false, PageIndex = 1, PageSize = 5 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los menues.", result.Message);
            Assert.Contains("list error", result.Errors);
            _menuRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenMenuIsValid_InyectsAuditFieldsAndReturnsId()
        {
            var menu = CreateMenu();
            Menu? capturedMenu = null;
            var before = DateTime.UtcNow;

            _menuRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Menu>()))
                .Callback<Menu>(value => capturedMenu = value)
                .ReturnsAsync(14);

            var result = await _service.AddAsync(menu, 61);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(14, result.Data);
            Assert.Equal("Menu agregado exitosamente.", result.Message);
            Assert.NotNull(capturedMenu);
            Assert.Equal(61, capturedMenu.IdUsuarioCreacion);
            Assert.Equal(61, capturedMenu.IdUsuarioModificacion);
            Assert.InRange(capturedMenu.FechaCreacion, before, after);
            Assert.InRange(capturedMenu.FechaModificacion, before, after);
            _menuRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Menu>()), Times.Once);
            _menuRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Menu>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _menuRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Menu>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateMenu(), 61);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el menu.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _menuRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Menu>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenMenuIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var menu = CreateMenu(19);
            Menu? capturedMenu = null;
            var before = DateTime.UtcNow;

            _menuRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Menu>()))
                .Callback<Menu>(value => capturedMenu = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(menu, 72);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Menu actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedMenu);
            Assert.Equal(72, capturedMenu.IdUsuarioModificacion);
            Assert.InRange(capturedMenu.FechaModificacion, before, after);
            _menuRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Menu>()), Times.Once);
            _menuRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _menuRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Menu>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateMenu(19), 72);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el menu.", result.Message);
            Assert.Contains("update error", result.Errors);
            _menuRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Menu>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesMenu_ReturnsSuccess()
        {
            _menuRepositoryMock
                .Setup(x => x.DeleteAsync(25))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(25, 72);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Menu eliminado exitosamente.", result.Message);
            _menuRepositoryMock.Verify(x => x.DeleteAsync(25), Times.Once);
            _menuRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _menuRepositoryMock
                .Setup(x => x.DeleteAsync(25))
                .ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(25, 72);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el menu.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _menuRepositoryMock.Verify(x => x.DeleteAsync(25), Times.Once);
        }

        [Fact]
        public async Task GetMenusByUser_WhenMenusExist_ReturnsSuccess()
        {
            var menus = new List<Menu> { CreateMenu(31), CreateMenu(32) };
            var perfil = new Perfil { Id = 9, Menus = menus };
            var usuario = new Usuario { Id = 8, Perfiles = new List<Perfil> { perfil } };

            _usuarioRepositoryMock
                .Setup(x => x.GetByIdAsync(8))
                .ReturnsAsync(usuario);

            var result = await _service.GetMenusByUserAsync(8, 9);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.ToList().Count);
            Assert.Equal("Información de los menues del usuario obtenida exitosamente.", result.Message);
            _usuarioRepositoryMock.Verify(x => x.GetByIdAsync(8), Times.Once);
        }

        [Fact]
        public async Task GetMenusByUser_WhenBusinessThrows_ReturnsFailure()
        {
            _usuarioRepositoryMock
                .Setup(x => x.GetByIdAsync(8))
                .ThrowsAsync(new Exception("user menus error"));

            var result = await _service.GetMenusByUserAsync(8, 9);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los menues del usuario.", result.Message);
            Assert.Contains("user menus error", result.Errors);
            _usuarioRepositoryMock.Verify(x => x.GetByIdAsync(8), Times.Once);
        }

        [Fact]
        public async Task GetMenusByProfile_WhenMenusExist_ReturnsSuccess()
        {
            var menus = new List<Menu> { CreateMenu(41), CreateMenu(42) };
            var perfil = new Perfil { Id = 3, Menus = menus };

            _perfilRepositoryMock
                .Setup(x => x.GetByIdAsync(3))
                .ReturnsAsync(perfil);

            var result = await _service.GetMenusByProfileAsync(3);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.ToList().Count);
            Assert.Equal("Información de los menues del perfil obtenida exitosamente.", result.Message);
            _perfilRepositoryMock.Verify(x => x.GetByIdAsync(3), Times.Once);
        }

        [Fact]
        public async Task GetMenusByProfile_WhenBusinessThrows_ReturnsFailure()
        {
            _perfilRepositoryMock
                .Setup(x => x.GetByIdAsync(3))
                .Throws(new Exception("profile menus error"));

            var result = await _service.GetMenusByProfileAsync(3);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los menues del perfil.", result.Message);
            Assert.Contains("profile menus error", result.Errors);
            _perfilRepositoryMock.Verify(x => x.GetByIdAsync(3), Times.Once);
        }

        private static Menu CreateMenu(int id = 1)
        {
            return new Menu
            {
                Id = id,
                Nombre = "Menu Demo",
                Ruta = "/demo",
                Imagen = "demo.png"
            };
        }
    }
}
