using System;
using System.Collections.Generic;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.NEG.Services;
using HARD.CORE.OBJ;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HARD.CORE.NEG.Tests.Services
{
    public class ClienteServiceTests
    {
        private readonly Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Cliente, BaseFilter, int>> _clienteRepositoryMock;
        private readonly Mock<ILogger<ClienteService>> _loggerMock;
        private readonly ClienteService _service;

        public ClienteServiceTests()
        {
            _clienteRepositoryMock = new Mock<HARD.CORE.DAT.Interfaces.IRepositoryBase<Cliente, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<ClienteService>>();
            _service = new ClienteService(_loggerMock.Object, _clienteRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenClientExists_ReturnsSuccess()
        {
            var cliente = CreateCliente(4);
            _clienteRepositoryMock
                .Setup(x => x.GetByIdAsync(4))
                .ReturnsAsync(cliente);

            var result = await _service.GetByIdAsync(4);

            Assert.True(result.Success);
            Assert.Equal(cliente, result.Data);
            Assert.Equal("Información del cliente obtenida exitosamente.", result.Message);
            _clienteRepositoryMock.Verify(x => x.GetByIdAsync(4), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _clienteRepositoryMock
                .Setup(x => x.GetByIdAsync(4))
                .ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(4);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del cliente.", result.Message);
            Assert.Contains("get error", result.Errors);
            _clienteRepositoryMock.Verify(x => x.GetByIdAsync(4), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenClientsExist_ReturnsSuccess()
        {
            var clientes = new List<Cliente> { CreateCliente(1), CreateCliente(2) };

            _clienteRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 3 &&
                    f.PageSize == 15 &&
                    f.IdMaster == 7 &&
                    f.IdDetail == 9 &&
                    f.Activo == true)))
                .ReturnsAsync(new HARD.CORE.OBJ.Models.PagedResult<Cliente> { Data = clientes, TotalCount = 2, PageIndex = 3, PageSize = 15 });

            var filter = new BaseFilter { Activo = true, IdMaster = 7, IdDetail = 9, PageIndex = 3, PageSize = 15 };
            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _clienteRepositoryMock.Verify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 3 &&
                f.PageSize == 15 &&
                f.IdMaster == 7 &&
                f.IdDetail == 9 &&
                f.Activo == true)), Times.Once);
            _clienteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _clienteRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .ThrowsAsync(new Exception("list error"));

            var filter = new BaseFilter { Activo = false, IdMaster = 1, IdDetail = 2, PageIndex = 1, PageSize = 10 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los clientes.", result.Message);
            Assert.Contains("list error", result.Errors);
            _clienteRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenClientIsValid_InyectsAuditFieldsAndReturnsIdAsync()
        {
            var cliente = CreateCliente();
            Cliente? capturedCliente = null;
            var before = DateTime.UtcNow;

            _clienteRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Cliente>()))
                .Callback<Cliente>(value => capturedCliente = value)
                .ReturnsAsync(21);

            var result = await _service.AddAsync(cliente, 50);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(21, result.Data);
            Assert.Equal("Cliente agregado exitosamente.", result.Message);
            Assert.NotNull(capturedCliente);
            Assert.Equal(50, capturedCliente.IdUsuarioCreacion);
            Assert.Equal(50, capturedCliente.IdUsuarioModificacion);
            Assert.InRange(capturedCliente.FechaCreacion, before, after);
            Assert.InRange(capturedCliente.FechaModificacion, before, after);
            _clienteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>()), Times.Once);
            _clienteRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _clienteRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Cliente>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateCliente(), 50);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el cliente.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _clienteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenClientIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var cliente = CreateCliente(9);
            Cliente? capturedCliente = null;
            var before = DateTime.UtcNow;

            _clienteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Cliente>()))
                .Callback<Cliente>(value => capturedCliente = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(cliente, 88);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Cliente actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedCliente);
            Assert.Equal(88, capturedCliente.IdUsuarioModificacion);
            Assert.InRange(capturedCliente.FechaModificacion, before, after);
            _clienteRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cliente>()), Times.Once);
            _clienteRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _clienteRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Cliente>()))
                .Throws(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateCliente(9), 88);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el cliente.", result.Message);
            Assert.Contains("update error", result.Errors);
            _clienteRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesClient_ReturnsSuccess()
        {
            _clienteRepositoryMock
                .Setup(x => x.DeleteAsync(17))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(17, 88);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Cliente eliminado exitosamente.", result.Message);
            _clienteRepositoryMock.Verify(x => x.DeleteAsync(17), Times.Once);
            _clienteRepositoryMock.Verify(x => x.AddAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _clienteRepositoryMock
                .Setup(x => x.DeleteAsync(17))
                .Throws(new Exception("delete error"));

            var result = await _service.DeleteAsync(17, 88);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el cliente.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _clienteRepositoryMock.Verify(x => x.DeleteAsync(17), Times.Once);
        }

        private static Cliente CreateCliente(int id = 1)
        {
            return new Cliente
            {
                Id = id,
                RFC = "XAXX010101000",
                RazonSocial = "Cliente Demo"
            };
        }
    }
}
