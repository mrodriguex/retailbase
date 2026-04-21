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
    public class CompanyServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Company, BaseFilter, int>> _companyRepositoryMock;
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<User, BaseFilter, int>> _userRepositoryMock;
        private readonly Mock<ILogger<CompanyService>> _loggerMock;
        private readonly CompanyService _service;

        public CompanyServiceTests()
        {
            _companyRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Company, BaseFilter, int>>();
            _userRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<User, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<CompanyService>>();
            _service = new CompanyService(_loggerMock.Object, _companyRepositoryMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenCompanyExists_ReturnsSuccess()
        {
            var company = CreateCompany(3);
            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(3))
                .ReturnsAsync(company);

            var result = await _service.GetByIdAsync(3);

            Assert.True(result.Success);
            Assert.Equal(company, result.Data);
            Assert.Equal("Información del company obtenida exitosamente.", result.Message);
            _companyRepositoryMock.Viewify(x => x.GetByIdAsync(3), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _companyRepositoryMock
                .Setup(x => x.GetByIdAsync(3))
                .Throws(new Exception("get error"));

            var result = await _service.GetByIdAsync(3);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del company.", result.Message);
            Assert.Contains("get error", result.Errors);
            _companyRepositoryMock.Viewify(x => x.GetByIdAsync(3), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenCompaniesExist_ReturnsSuccess()
        {
            var companys = new List<Company> { CreateCompany(1), CreateCompany(2) };
            var filter = new BaseFilter { Enabled = true, IdMaster = 5, IdDetail = 6, PageIndex = 2, PageSize = 25 };

            _companyRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 2 &&
                    f.PageSize == 25 &&
                    f.IdMaster == 5 &&
                    f.IdDetail == 6 &&
                    f.Enabled == true)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<Company> { Data = companys, TotalCount = 2, PageIndex = 2, PageSize = 25 });

            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _companyRepositoryMock.Viewify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 2 &&
                f.PageSize == 25 &&
                f.IdMaster == 5 &&
                f.IdDetail == 6 &&
                f.Enabled == true)), Times.Once);
            _companyRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Company>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _companyRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .Throws(new Exception("list error"));

            var filter = new BaseFilter { Enabled = false, IdMaster = 5, IdDetail = 6, PageIndex = 1, PageSize = 10 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los companyes.", result.Message);
            Assert.Contains("list error", result.Errors);
            _companyRepositoryMock.Viewify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenCompanyIsValid_InyectsAuditFieldsAndReturnsId()
        {
            var company = CreateCompany();
            Company? capturedCompany = null;
            var before = DateTime.UtcNow;

            _companyRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Company>()))
                .Callback<Company>(value => capturedCompany = value)
                .ReturnsAsync(31);

            var result = await _service.AddAsync(company, 64);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(31, result.Data);
            Assert.Equal("Company agregado exitosamente.", result.Message);
            Assert.NotNull(capturedCompany);
            Assert.Equal(64, capturedCompany.IdUserCreation);
            Assert.Equal(64, capturedCompany.IdUserModification);
            Assert.InRange(capturedCompany.DateTimeCreation, before, after);
            Assert.InRange(capturedCompany.DateTimeModification, before, after);
            _companyRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Company>()), Times.Once);
            _companyRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Company>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _companyRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Company>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateCompany(), 64);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el company.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _companyRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Company>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenCompanyIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var company = CreateCompany(11);
            Company? capturedCompany = null;
            var before = DateTime.UtcNow;

            _companyRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Company>()))
                .Callback<Company>(value => capturedCompany = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(company, 70);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Company actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedCompany);
            Assert.Equal(70, capturedCompany.IdUserModification);
            Assert.InRange(capturedCompany.DateTimeModification, before, after);
            _companyRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Company>()), Times.Once);
            // Removed invalid GetCompaniesByUserAsync verification (not a repository method)
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _companyRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Company>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateCompany(11), 70);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el company.", result.Message);
            Assert.Contains("update error", result.Errors);
            _companyRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Company>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesCompany_ReturnsSuccess()
        {
            _companyRepositoryMock
                .Setup(x => x.DeleteAsync(15))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(15, 70);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Company eliminado exitosamente.", result.Message);
            _companyRepositoryMock.Viewify(x => x.DeleteAsync(15), Times.Once);
            _companyRepositoryMock.Viewify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _companyRepositoryMock
                .Setup(x => x.DeleteAsync(15))
                .Throws(new Exception("delete error"));

            var result = await _service.DeleteAsync(15, 70);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el company.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _companyRepositoryMock.Viewify(x => x.DeleteAsync(15), Times.Once);
        }

        private static Company CreateCompany(int id = 1)
        {
            return new Company
            {
                Id = id,
                TAXID = "AAA010101AAA",
                LegalName = "Company Demo"
            };
        }
    }
}
