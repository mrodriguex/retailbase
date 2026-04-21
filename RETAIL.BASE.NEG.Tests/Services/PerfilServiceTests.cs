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
    public class RoleServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Role, BaseFilter, int>> _roleRepositoryMock;
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<User, BaseFilter, int>> _userRepositoryMock;
        private readonly Mock<ILogger<RoleService>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly RoleService _service;

        public RoleServiceTests()
        {
            _roleRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<Role, BaseFilter, int>>();
            _userRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<User, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<RoleService>>();
            _configurationMock = new Mock<IConfiguration>();
            _service = new RoleService(_loggerMock.Object, _roleRepositoryMock.Object, _userRepositoryMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task GetById_WhenProfileExists_ReturnsSuccess()
        {
            var role = CreateRole(5);
            _roleRepositoryMock
                .Setup(x => x.GetByIdAsync(5))
                .ReturnsAsync(role);

            var result = await _service.GetByIdAsync(5);

            Assert.True(result.Success);
            Assert.Equal(role, result.Data);
            Assert.Equal("Información del role obtenida exitosamente.", result.Message);
            _roleRepositoryMock.Viewify(x => x.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _roleRepositoryMock
                .Setup(x => x.GetByIdAsync(5))
                .ThrowsAsync(new Exception("get error"));

            var result = await _service.GetByIdAsync(5);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del role.", result.Message);
            Assert.Contains("get error", result.Errors);
            _roleRepositoryMock.Viewify(x => x.GetByIdAsync(5), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenProfilesExist_ReturnsSuccess()
        {
            var roles = new List<Role> { CreateRole(1), CreateRole(2) };
            _roleRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 4 &&
                    f.PageSize == 30 &&
                    f.Enabled == true)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<Role> { Data = roles, TotalCount = 2, PageIndex = 4, PageSize = 30 });

            var filter = new BaseFilter { Enabled = true, PageIndex = 4, PageSize = 30 };
            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _roleRepositoryMock.Viewify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 4 &&
                f.PageSize == 30 &&
                f.Enabled == true)), Times.Once);
            _roleRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _roleRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .ThrowsAsync(new Exception("list error"));

            var filter = new BaseFilter { Enabled = false, PageIndex = 1, PageSize = 10 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los roles.", result.Message);
            Assert.Contains("list error", result.Errors);
            _roleRepositoryMock.Viewify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenProfileIsValid_InyectsAuditFieldsAndReturnsId()
        {
            var role = CreateRole();
            Role? capturedRole = null;
            var before = DateTime.UtcNow;

            _roleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Role>()))
                .Callback<Role>(value => capturedRole = value)
                .ReturnsAsync(99);

            var result = await _service.AddAsync(role, 41);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(99, result.Data);
            Assert.Equal("Role agregado exitosamente.", result.Message);
            Assert.NotNull(capturedRole);
            Assert.Equal(41, capturedRole.IdUserCreation);
            Assert.Equal(41, capturedRole.IdUserModification);
            Assert.InRange(capturedRole.DateTimeCreation, before, after);
            Assert.InRange(capturedRole.DateTimeModification, before, after);
            _roleRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Role>()), Times.Once);
            _roleRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            _roleRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<Role>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(CreateRole(), 41);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el role.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _roleRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenProfileIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var role = CreateRole(18);
            Role? capturedRole = null;
            var before = DateTime.UtcNow;

            _roleRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Role>()))
                .Callback<Role>(value => capturedRole = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(role, 77);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Role actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedRole);
            Assert.Equal(77, capturedRole.IdUserModification);
            Assert.InRange(capturedRole.DateTimeModification, before, after);
            _roleRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Role>()), Times.Once);
            _roleRepositoryMock.Viewify(x => x.AddAsync(It.IsAny<Role>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            _roleRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<Role>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(CreateRole(18), 77);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el role.", result.Message);
            Assert.Contains("update error", result.Errors);
            _roleRepositoryMock.Viewify(x => x.UpdateAsync(It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesProfile_ReturnsSuccess()
        {
            _roleRepositoryMock
                .Setup(x => x.DeleteAsync(22))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(22, 77);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Role eliminado exitosamente.", result.Message);
            _roleRepositoryMock.Viewify(x => x.DeleteAsync(22), Times.Once);
            _roleRepositoryMock.Viewify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _roleRepositoryMock
                .Setup(x => x.DeleteAsync(22))
                .ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(22, 77);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el role.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _roleRepositoryMock.Viewify(x => x.DeleteAsync(22), Times.Once);
        }

        [Fact]
        public async Task GetUserProfiles_WhenProfilesExist_ReturnsSuccess()
        {
            var user = new User { Id = 12, Roles = new List<Role> { CreateRole(10), CreateRole(11) } };
            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(12))
                .ReturnsAsync(user);

            var result = await _service.GetUserProfilesAsync(12);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal("Información de los roles del user obtenida exitosamente.", result.Message);
            _userRepositoryMock.Viewify(x => x.GetByIdAsync(12), Times.Once);
        }

        [Fact]
        public async Task GetUserProfiles_WhenBusinessThrows_ReturnsFailure()
        {
            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(12))
                .ThrowsAsync(new Exception("profiles error"));

            var result = await _service.GetUserProfilesAsync(12);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los roles del user.", result.Message);
            Assert.Contains("profiles error", result.Errors);
            _userRepositoryMock.Viewify(x => x.GetByIdAsync(12), Times.Once);
        }

        private static Role CreateRole(int id = 1)
        {
            return new Role
            {
                Id = id,
                Name = "Role Demo",
                Description = "Role de prueba"
            };
        }
    }
}
