using System;
using HARD.CORE.NEG.Interfaces;
using HARD.CORE.NEG.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HARD.CORE.NEG.Tests.Services
{
    public class CryptographerServiceTests
    {
        private readonly Mock<ICryptographerB> _cryptographerMock;
        private readonly Mock<ILogger<CryptographerService>> _loggerMock;
        private readonly CryptographerService _service;

        public CryptographerServiceTests()
        {
            _cryptographerMock = new Mock<ICryptographerB>();
            _loggerMock = new Mock<ILogger<CryptographerService>>();
            _service = new CryptographerService(_cryptographerMock.Object, _loggerMock.Object);
        }

        [Fact]
        public void CreateHash_WhenInputIsNull_ReturnsFailure()
        {
            var result = _service.CreateHash(null);

            Assert.False(result.Success);
            Assert.Contains("input", string.Join(" ", result.Errors), StringComparison.OrdinalIgnoreCase);
            _cryptographerMock.Verify(x => x.CreateHash(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CreateHash_WhenInputIsValid_ReturnsSuccessAndHash()
        {
            var encodedInput = "hola%20mundo";
            var decodedInput = "hola mundo";
            var expectedHash = "HASH123";

            _cryptographerMock
                .Setup(x => x.CreateHash(decodedInput))
                .Returns(expectedHash);

            var result = _service.CreateHash(encodedInput);

            Assert.True(result.Success);
            Assert.Equal(expectedHash, result.Data);
            _cryptographerMock.Verify(x => x.CreateHash(decodedInput), Times.Once);
        }

        [Fact]
        public void CreateHash_WhenCryptographerThrows_ReturnsFailure()
        {
            _cryptographerMock
                .Setup(x => x.CreateHash(It.IsAny<string>()))
                .Throws(new Exception("boom"));

            var result = _service.CreateHash("abc");

            Assert.False(result.Success);
            Assert.Contains("boom", string.Join(" ", result.Errors), StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CompareHash_WhenInputIsNull_ReturnsFailure()
        {
            var result = _service.CompareHash(null, "hash");

            Assert.False(result.Success);
            Assert.Contains("input", string.Join(" ", result.Errors), StringComparison.OrdinalIgnoreCase);
            _cryptographerMock.Verify(x => x.CompareHash(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CompareHash_WhenHashIsNull_ReturnsFailure()
        {
            var result = _service.CompareHash("input", null);

            Assert.False(result.Success);
            Assert.Contains("hash", string.Join(" ", result.Errors), StringComparison.OrdinalIgnoreCase);
            _cryptographerMock.Verify(x => x.CompareHash(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void CompareHash_WhenValuesAreValid_ReturnsSuccess()
        {
            var encodedInput = "hola%20mundo";
            var encodedHash = "abc%2B123";
            var decodedInput = "hola mundo";
            var decodedHash = "abc+123";

            _cryptographerMock
                .Setup(x => x.CompareHash(decodedInput, decodedHash))
                .Returns(true);

            var result = _service.CompareHash(encodedInput, encodedHash);

            Assert.True(result.Success);
            Assert.True(result.Data);
            _cryptographerMock.Verify(x => x.CompareHash(decodedInput, decodedHash), Times.Once);
        }

        [Fact]
        public void CompareHash_WhenCryptographerThrows_ReturnsFailure()
        {
            _cryptographerMock
                .Setup(x => x.CompareHash(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("compare error"));

            var result = _service.CompareHash("abc", "def");

            Assert.False(result.Success);
            Assert.Contains("compare error", string.Join(" ", result.Errors), StringComparison.OrdinalIgnoreCase);
        }
    }
}