using RETAIL.BASE.NEG.Services;
using RETAIL.BASE.OBJ;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace RETAIL.BASE.NEG.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<User, BaseFilter, int>> _userRepositoryMock;
        private readonly Mock<ILogger<UserService>> _loggerMock;
        private readonly UserService _service;

        public UserServiceTests()
        {
            _userRepositoryMock = new Mock<RETAIL.BASE.DAT.Interfaces.IRepositoryBase<User, BaseFilter, int>>();
            _loggerMock = new Mock<ILogger<UserService>>();
            _service = new UserService(_loggerMock.Object, _userRepositoryMock.Object);
        }

        [Fact]
        public async Task GetById_WhenUserExists_ReturnsSuccessfulResult()
        {
            var user = CreateUser(10);
            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .ReturnsAsync(user);

            var result = await _service.GetByIdAsync(10);

            Assert.True(result.Success);
            Assert.Equal(user, result.Data);
            Assert.Equal("Información del user obtenida exitosamente.", result.Message);
            Assert.Empty(result.Errors);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
        }

        [Fact]
        public async Task GetById_WhenBusinessThrows_ReturnsFailure()
        {
            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(10))
                .Throws(new InvalidOperationException("db error"));

            var result = await _service.GetByIdAsync(10);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información del user.", result.Message);
            Assert.Contains("db error", result.Errors);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(10), Times.Once);
        }

        [Fact]
        public async Task GetAll_WhenUsersExist_ReturnsSuccessfulResult()
        {
            var users = new List<User> { CreateUser(1), CreateUser(2) };

            _userRepositoryMock
                .Setup(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                    f.PageIndex == 2 &&
                    f.PageSize == 5 &&
                    f.Enabled == true)))
                .ReturnsAsync(new RETAIL.BASE.OBJ.Models.PagedResult<User> { Data = users, TotalCount = 2, PageIndex = 2, PageSize = 5 });

            var filter = new BaseFilter { Enabled = true, PageIndex = 2, PageSize = 5 };
            var result = await _service.GetAllAsync(filter);

            Assert.True(result.Success);
            Assert.Equal(2, result.Data.Data.Count());
            Assert.Equal("Información obtenida exitosamente.", result.Message);
            _userRepositoryMock.Verify(x => x.GetAllAsync(It.Is<BaseFilter>(f =>
                f.PageIndex == 2 &&
                f.PageSize == 5 &&
                f.Enabled == true)), Times.Once);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task GetAll_WhenBusinessThrows_ReturnsFailure()
        {
            _userRepositoryMock
                .Setup(x => x.GetAllAsync(It.IsAny<BaseFilter>()))
                .Throws(new Exception("query failed"));

            var filter = new BaseFilter { Enabled = false, PageIndex = 1, PageSize = 20 };
            var result = await _service.GetAllAsync(filter);

            Assert.False(result.Success);
            Assert.Null(result.Data);
            Assert.Equal("Error al obtener la información de los users.", result.Message);
            Assert.Contains("query failed", result.Errors);
            _userRepositoryMock.Verify(x => x.GetAllAsync(It.IsAny<BaseFilter>()), Times.Once);
        }

        [Fact]
        public async Task Add_WhenUserIsValid_InyectsAuditFieldsAndReturnsId()
        {
            var user = CreateUser();
            User? capturedUser = null;
            var before = DateTime.UtcNow;

            _userRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .Callback<User>(value => capturedUser = value)
                .ReturnsAsync(55);

            var result = await _service.AddAsync(user, 99);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.Equal(55, result.Data);
            Assert.Equal("User agregado exitosamente.", result.Message);
            Assert.NotNull(capturedUser);
            Assert.Equal(99, capturedUser.IdUserCreation);
            Assert.Equal(99, capturedUser.IdUserModification);
            Assert.InRange(capturedUser.DateTimeCreation, before, after);
            Assert.InRange(capturedUser.DateTimeModification, before, after);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        }

        [Fact]
        public async Task Add_WhenBusinessThrows_ReturnsFailure()
        {
            var user = CreateUser();

            _userRepositoryMock
                .Setup(x => x.AddAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("insert error"));

            var result = await _service.AddAsync(user, 77);

            Assert.False(result.Success);
            Assert.Equal(0, result.Data);
            Assert.Equal("Error al agregar el user.", result.Message);
            Assert.Contains("insert error", result.Errors);
            _userRepositoryMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenUserIsValid_InyectsAuditFieldsAndReturnsSuccess()
        {
            var user = CreateUser(8);
            User? capturedUser = null;
            var before = DateTime.UtcNow;

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(8))
                .ReturnsAsync(CreateUser(8));

            _userRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(value => capturedUser = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(user, 44);
            var after = DateTime.UtcNow;

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("User actualizado exitosamente.", result.Message);
            Assert.NotNull(capturedUser);
            Assert.Equal(44, capturedUser.IdUserModification);
            Assert.InRange(capturedUser.DateTimeModification, before, after);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(8), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
            _userRepositoryMock.Verify(x => x.DeleteAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Update_WhenBusinessThrows_ReturnsFailure()
        {
            var user = CreateUser(8);

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(8))
                .ReturnsAsync(CreateUser(8));

            _userRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .ThrowsAsync(new Exception("update error"));

            var result = await _service.UpdateAsync(user, 44);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al actualizar el user.", result.Message);
            Assert.Contains("update error", result.Errors);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(8), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Update_WhenNameAndFirstNameAreMissing_PreservesCurrentValuesIndependently()
        {
            var user = new User
            {
                Id = 8,
                Name = "",
                FirstName = "",
                UserName = "user.test",
                Email = "user@test.com"
            };

            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(8))
                .ReturnsAsync(new User
                {
                    Id = 8,
                    Name = "Name actual",
                    FirstName = "login.actual",
                    Password = "pwd-hash"
                });

            User? capturedUser = null;
            _userRepositoryMock
                .Setup(x => x.UpdateAsync(It.IsAny<User>()))
                .Callback<User>(value => capturedUser = value)
                .ReturnsAsync(true);

            var result = await _service.UpdateAsync(user, 44);

            Assert.True(result.Success);
            Assert.NotNull(capturedUser);
            Assert.Equal("Name actual", capturedUser!.Name);
            Assert.Equal("login.actual", capturedUser.FirstName);
            Assert.Equal("pwd-hash", capturedUser.Password);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(8), Times.Once);
            _userRepositoryMock.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task Delete_WhenBusinessDeletesUser_ReturnsSuccess()
        {
            _userRepositoryMock
                .Setup(x => x.DeleteAsync(13))
                .ReturnsAsync(true);

            var result = await _service.DeleteAsync(13, 7);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("User eliminado exitosamente.", result.Message);
            _userRepositoryMock.Verify(x => x.DeleteAsync(13), Times.Once);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact]
        public async Task Delete_WhenBusinessThrows_ReturnsFailure()
        {
            _userRepositoryMock
                .Setup(x => x.DeleteAsync(13))
                .ThrowsAsync(new Exception("delete error"));

            var result = await _service.DeleteAsync(13, 7);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al eliminar el user.", result.Message);
            Assert.Contains("delete error", result.Errors);
            _userRepositoryMock.Verify(x => x.DeleteAsync(13), Times.Once);
        }

        [Fact]
        public async Task Exists_WhenUserExists_ReturnsSuccess()
        {
            var user = CreateUser(13);
            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(13))
                .ReturnsAsync(user);

            var result = await _service.ExistsAsync(13);

            Assert.True(result.Success);
            Assert.True(result.Data);
            Assert.Equal("Viewificación de existencia del user realizada exitosamente.", result.Message);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(13), Times.Once);
        }

        [Fact]
        public async Task Exists_WhenBusinessThrows_ReturnsFailure()
        {
            _userRepositoryMock
                .Setup(x => x.GetByIdAsync(13))
                .ThrowsAsync(new Exception("exists error"));

            var result = await _service.ExistsAsync(13);

            Assert.False(result.Success);
            Assert.False(result.Data);
            Assert.Equal("Error al verificar la existencia del user.", result.Message);
            Assert.Contains("exists error", result.Errors);
            _userRepositoryMock.Verify(x => x.GetByIdAsync(13), Times.Once);
        }

        private static User CreateUser(int id = 1)
        {
            return new User
            {
                Id = id,
                UserName = "user.test",
                FirstName = "User",
                LastNameFather = "Prueba",
                LastNameMother = "Demo",
                Email = "user@test.com"
            };
        }
    }
}
