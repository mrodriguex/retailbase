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
    public class PerfilServiceTests
    {
        private readonly Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Perfil, BaseFilter, int>> _perfilRepositoryMock;
        private readonly Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Usuario, BaseFilter, int>> _usuarioRepositoryMock;
        private readonly Mock<ILogger<PerfilService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly PerfilService _service;

        public PerfilServiceTests()
        {
            _perfilRepositoryMock = new Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Perfil, BaseFilter, int>>();
            _usuarioRepositoryMock = new Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Usuario, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<PerfilService>>();
            _configurationMock = new Mock<IConfiguration>();
            _service = new PerfilService(_loggerMock.Object, _perfilRepositoryMock.Object, _usuarioRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task GetById_WhenProfileExists_ReturnsSuccess()
        {
            var perfil = CreatePerfil(5);
            _perfilRepositoryMock
                .Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(perfil);

            var result = await _service.GetByIdAsync(5);

            Assert.True(result.Success);
            Assert.Equal(perfil, result.Data);
            Assert.Equal("Información del perfil obtenida exitosamente.", result.Message);
            _perfilRepositoryMock.Verify(x => x.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _perfilRepositoryMock
                .Setup(x => x.GetByIdAsync(5))
                .ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(5);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del perfil.", result.Message);
            Assert.Contains("get error", result.Errors);
            _perfilRepositoryMock.Verify(x => x.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenProfilesExist_ReturnsSuccess()
        {
            var perfiles = new List<Perfil> { CreatePerfil(1), CreatePerfil(2) };
            _perfilRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 4 &&
                    f.PageSize == 30 &&
                    f.Activo == true)))
                .ReturnsAsync(new HARD.CORE.OBJ.Models.PagedResult<Perfil> { Data = perfiles, TotalCount = 2, PageIndex = 4, PageSize = 30 });

            var filter = new BaseFilter { Activo = true, PageIndex = 4, PageSize = 30 };
            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _perfilRepositoryMock.Verify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 4 &&
                f.PageSize == 30 &&
                f.Activo == true)), Times.Once);
            _perfilRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Perfil>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _perfilRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .ThrowsAsync(new Exception("list error"));

            var filter = new BaseFilter { Activo = false, PageIndex = 1, PageSize = 10 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los perfiles.", result.Message);
            Assert.Contains("list error", result.Errors);
            _perfilRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenProfileIsValid_InyectsAuditFieldsAndReturnsId()
        {
            var perfil = CreatePerfil();
            Perfil? capturedPerfil = null;
            var before = DateTime.UtcNow;

            _perfilRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Perfil>()))
                .Callback<Perfil>(value => capturedPerfil = value)
                .ReturnsAsync(99);

            var result = await _service.AddAsync(perfil, 41);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(99, result.Data);
            Assert.Equal("Perfil agregado exitosamente.", result.Message);
            Assert.NotNull(capturedPerfil);
            Assert.Equal(41, capturedPerfil.IdUsuarioCreacion);
            Assert.Equal(41, capturedPerfil.IdUsuarioModificacion);
            Assert.InRange(capturedPerfil.FechaCreacion, before, after);
            Assert.InRange(capturedPerfil.FechaModificacion, before, after);
            _perfilRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Perfil>()), Times.Once);
            _perfilRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Perfil>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _perfilRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Perfil>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreatePerfil(), 41);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el perfil.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _perfilRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Perfil>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenProfileIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var perfil = CreatePerfil(18);
            Perfil? capturedPerfil = null;
            var before = DateTime.UtcNow;

            _perfilRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Perfil>()))
                .Callback<Perfil>(value => capturedPerfil = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(perfil, 77);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Perfil actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedPerfil);
            Assert.Equal(77, capturedPerfil.IdUsuarioModificacion);
            Assert.InRange(capturedPerfil.FechaModificacion, before, after);
            _perfilRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Perfil>()), Times.Once);
            _perfilRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Perfil>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _perfilRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Perfil>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreatePerfil(18), 77);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el perfil.", result.Message);
            Assert.Contains("update error", result.Errors);
            _perfilRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Perfil>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesProfile_ReturnsSuccess()
        {
            _perfilRepositoryMock
                .Setup(x => x.DeleteAsync(22))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(22, 77);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Perfil eliminado exitosamente.", result.Message);
            _perfilRepositoryMock.Verify(x => x.DeleteAsync(22), Times.Once);
            _perfilRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _perfilRepositoryMock
                .Setup(x => x.DeleteAsync(22))
                .ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(22, 77);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el perfil.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _perfilRepositoryMock.Verify(x => x.DeleteAsync(22), Times.Once);
        }

        [Fact]
        public async Task GetUserProfiles_WhenProfilesExist_ReturnsSuccess()
        {
            var usuario = new Usuario { Id = 12, Perfiles = new List<Perfil> { CreatePerfil(10), CreatePerfil(11) } };
            _usuarioRepositoryMock
                .Setup(x => x.GetByIdAsync(12))
                .ReturnsAsync(usuario);

            var result = await _service.GetUserProfilesAsync(12);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal("Información de los perfiles del usuario obtenida exitosamente.", result.Message);
            _usuarioRepositoryMock.Verify(x => x.GetByIdAsync(12), Times.Once);
        }

        [Fact]
        public async Task GetUserProfiles_WhenBusinessThrows_ReturnsFailure()
        {
            _usuarioRepositoryMock
                .Setup(x => x.GetByIdAsync(12))
                .ThrowsAsync(new Exception("profiles error"));

            var result = await _service.GetUserProfilesAsync(12);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los perfiles del usuario.", result.Message);
            Assert.Contains("profiles error", result.Errors);
            _usuarioRepositoryMock.Verify(x => x.GetByIdAsync(12), Times.Once);
        }

        private static Perfil CreatePerfil(int id = 1)
        {
            return new Perfil
            {
                Id = id,
                Nombre = "Perfil Demo",
                Descripcion = "Perfil de prueba"
            };
        }
    }
}
