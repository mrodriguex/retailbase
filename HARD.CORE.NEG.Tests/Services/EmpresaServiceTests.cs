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
    public class EmpresaServiceTests
    {
        private readonly Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Empresa, BaseFilter, int>> _empresaRepositoryMock;
        private readonly Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Usuario, BaseFilter, int>> _usuarioRepositoryMock;
        private readonly Mock<ILogger<EmpresaService>> _loggerMock;
        private readonly EmpresaService _service;

        public EmpresaServiceTests()
        {
            _empresaRepositoryMock = new Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Empresa, BaseFilter, int>>();
            _usuarioRepositoryMock = new Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Usuario, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<EmpresaService>>();
            _service = new EmpresaService(_loggerMock.Object, _empresaRepositoryMock.Object, _usuarioRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenCompanyExists_ReturnsSuccess()
        {
            var empresa = CreateEmpresa(3);
            _empresaRepositoryMock
                .Setup(x => x.GetByIdAsync(3))
                .ReturnsAsync(empresa);

            var result = await _service.GetByIdAsync(3);

            Assert.True(result.Success);
            Assert.Equal(empresa, result.Data);
            Assert.Equal("Información del empresa obtenida exitosamente.", result.Message);
            _empresaRepositoryMock.Verify(x => x.GetByIdAsync(3), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _empresaRepositoryMock
                .Setup(x => x.GetByIdAsync(3))
                .Throws(new Exception("get error"));

            var result = await _service.GetByIdAsync(3);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del empresa.", result.Message);
            Assert.Contains("get error", result.Errors);
            _empresaRepositoryMock.Verify(x => x.GetByIdAsync(3), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenCompaniesExist_ReturnsSuccess()
        {
            var empresas = new List<Empresa> { CreateEmpresa(1), CreateEmpresa(2) };
            var filter = new BaseFilter { Activo = true, IdMaster = 5, IdDetail = 6, PageIndex = 2, PageSize = 25 };

            _empresaRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 2 &&
                    f.PageSize == 25 &&
                    f.IdMaster == 5 &&
                    f.IdDetail == 6 &&
                    f.Activo == true)))
                .ReturnsAsync(new HARD.CORE.OBJ.Models.PagedResult<Empresa> { Data = empresas, TotalCount = 2, PageIndex = 2, PageSize = 25 });

            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _empresaRepositoryMock.Verify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 2 &&
                f.PageSize == 25 &&
                f.IdMaster == 5 &&
                f.IdDetail == 6 &&
                f.Activo == true)), Times.Once);
            _empresaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Empresa>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _empresaRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .Throws(new Exception("list error"));

            var filter = new BaseFilter { Activo = false, IdMaster = 5, IdDetail = 6, PageIndex = 1, PageSize = 10 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los empresaes.", result.Message);
            Assert.Contains("list error", result.Errors);
            _empresaRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenCompanyIsValid_InyectsAuditFieldsAndReturnsId()
        {
            var empresa = CreateEmpresa();
            Empresa? capturedEmpresa = null;
            var before = DateTime.UtcNow;

            _empresaRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Empresa>()))
                .Callback<Empresa>(value => capturedEmpresa = value)
                .ReturnsAsync(31);

            var result = await _service.AddAsync(empresa, 64);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(31, result.Data);
            Assert.Equal("Empresa agregado exitosamente.", result.Message);
            Assert.NotNull(capturedEmpresa);
            Assert.Equal(64, capturedEmpresa.IdUsuarioCreacion);
            Assert.Equal(64, capturedEmpresa.IdUsuarioModificacion);
            Assert.InRange(capturedEmpresa.FechaCreacion, before, after);
            Assert.InRange(capturedEmpresa.FechaModificacion, before, after);
            _empresaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Empresa>()), Times.Once);
            _empresaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Empresa>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _empresaRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Empresa>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateEmpresa(), 64);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el empresa.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _empresaRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Empresa>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenCompanyIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var empresa = CreateEmpresa(11);
            Empresa? capturedEmpresa = null;
            var before = DateTime.UtcNow;

            _empresaRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Empresa>()))
                .Callback<Empresa>(value => capturedEmpresa = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(empresa, 70);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Empresa actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedEmpresa);
            Assert.Equal(70, capturedEmpresa.IdUsuarioModificacion);
            Assert.InRange(capturedEmpresa.FechaModificacion, before, after);
            _empresaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Empresa>()), Times.Once);
            // Removed invalid GetCompaniesByUserAsync verification (not a repository method)
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _empresaRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Empresa>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateEmpresa(11), 70);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el empresa.", result.Message);
            Assert.Contains("update error", result.Errors);
            _empresaRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Empresa>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesCompany_ReturnsSuccess()
        {
            _empresaRepositoryMock
                .Setup(x => x.DeleteAsync(15))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(15, 70);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Empresa eliminado exitosamente.", result.Message);
            _empresaRepositoryMock.Verify(x => x.DeleteAsync(15), Times.Once);
            _empresaRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _empresaRepositoryMock
                .Setup(x => x.DeleteAsync(15))
                .Throws(new Exception("delete error"));

            var result = await _service.DeleteAsync(15, 70);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el empresa.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _empresaRepositoryMock.Verify(x => x.DeleteAsync(15), Times.Once);
        }

        private static Empresa CreateEmpresa(int id = 1)
        {
            return new Empresa
            {
                Id = id,
                RFC = "AAA010101AAA",
                RazonSocial = "Empresa Demo"
            };
        }
    }
}
