using System;
using System.Collections.Generic;
using RETAIL.BASE.NEG.Interfaces;
using RETAIL.BASE.NEG.Services;
using RETAIL.BASE.OBJ;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RETAIL.BASE.NEG.Tests.Services
{
    public class CustomerServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Customer, BaseFilter, int>> _customerRepositoryMock;
        private readonly Mock<ILogger<CustomerService>> _loggerMock;
        private readonly CustomerService _service;

        public CustomerServiceTests()
        {
            _customerRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Customer, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<CustomerService>>();
            _service = new CustomerService(_loggerMock.Object, _customerRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenCustomerxists_ReturnsSuccess()
        {
            var customer = CreateCustomer(4);
            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(4))
                .ReturnsAsync(customer);

            var result = await _service.GetByIdAsync(4);

            Assert.True(result.Success);
            Assert.Equal(customer, result.Data);
            Assert.Equal("Información del customer obtenida exitosamente.", result.Message);
            _customerRepositoryMock.Viewify(x => x.GetByIdAsync(4), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _customerRepositoryMock
                .Setup(x => x.GetByIdAsync(4))
                .ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(4);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del customer.", result.Message);
            Assert.Contains("get error", result.Errors);
            _customerRepositoryMock.Viewify(x => x.GetByIdAsync(4), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenClientsExist_ReturnsSuccess()
        {
            var customers = new List<Customer> { CreateCustomer(1), CreateCustomer(2) };

            _customerRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 3 &&
                    f.PageSize == 15 &&
                    f.IdMaster == 7 &&
                    f.IdDetail == 9 &&
                    f.Enabled == true)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<Customer> { Data = customers, TotalCount = 2, PageIndex = 3, PageSize = 15 });

            var filter = new BaseFilter { Enabled = true, IdMaster = 7, IdDetail = 9, PageIndex = 3, PageSize = 15 };
            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _customerRepositoryMock.Viewify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 3 &&
                f.PageSize == 15 &&
                f.IdMaster == 7 &&
                f.IdDetail == 9 &&
                f.Enabled == true)), Times.Once);
            _customerRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _customerRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .ThrowsAsync(new Exception("list error"));

            var filter = new BaseFilter { Enabled = false, IdMaster = 1, IdDetail = 2, PageIndex = 1, PageSize = 10 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los customers.", result.Message);
            Assert.Contains("list error", result.Errors);
            _customerRepositoryMock.Viewify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenClientIsValid_InyectsAuditFieldsAndReturnsIdAsync()
        {
            var customer = CreateCustomer();
            Customer? capturedCustomer = null;
            var before = DateTime.UtcNow;

            _customerRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Customer>()))
                .Callback<Customer>(value => capturedCustomer = value)
                .ReturnsAsync(21);

            var result = await _service.AddAsync(customer, 50);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(21, result.Data);
            Assert.Equal("Customer agregado exitosamente.", result.Message);
            Assert.NotNull(capturedCustomer);
            Assert.Equal(50, capturedCustomer.IdUserCreation);
            Assert.Equal(50, capturedCustomer.IdUserModification);
            Assert.InRange(capturedCustomer.DateTimeCreation, before, after);
            Assert.InRange(capturedCustomer.DateTimeModification, before, after);
            _customerRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
            _customerRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _customerRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Customer>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateCustomer(), 50);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el customer.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _customerRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenClientIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var customer = CreateCustomer(9);
            Customer? capturedCustomer = null;
            var before = DateTime.UtcNow;

            _customerRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
                .Callback<Customer>(value => capturedCustomer = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(customer, 88);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Customer actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedCustomer);
            Assert.Equal(88, capturedCustomer.IdUserModification);
            Assert.InRange(capturedCustomer.DateTimeModification, before, after);
            _customerRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Customer>()), Times.Once);
            _customerRepositoryMock.Viewify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _customerRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Customer>()))
                .Throws(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateCustomer(9), 88);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el customer.", result.Message);
            Assert.Contains("update error", result.Errors);
            _customerRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Customer>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesClient_ReturnsSuccess()
        {
            _customerRepositoryMock
                .Setup(x => x.DeleteAsync(17))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(17, 88);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Customer eliminado exitosamente.", result.Message);
            _customerRepositoryMock.Viewify(x => x.DeleteAsync(17), Times.Once);
            _customerRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Customer>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _customerRepositoryMock
                .Setup(x => x.DeleteAsync(17))
                .Throws(new Exception("delete error"));

            var result = await _service.DeleteAsync(17, 88);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el customer.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _customerRepositoryMock.Viewify(x => x.DeleteAsync(17), Times.Once);
        }

        private static Customer CreateCustomer(int id = 1)
        {
            return new Customer
            {
                Id = id,
                TAXID = "XAXX010101000",
                LegalName = "Customer Demo"
            };
        }
    }
}
