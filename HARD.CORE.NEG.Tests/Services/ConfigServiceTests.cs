using System;
using System.IO;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace HARD.CORE.NEG.Tests.Services
{
    public class ConfigServiceTests
    {
        private static readonly object SyncLock = new();
        private readonly Mock<ILogger<global::ConfigService>> _loggerMock;
        private readonly global::ConfigService _service;

        public ConfigServiceTests()
        {
            _loggerMock = new Mock<ILogger<global::ConfigService>>();
            _service = new global::ConfigService(_loggerMock.Object);
        }

        [Fact]
        public void GetAppSetting_WhenKeyExists_ReturnsValue()
        {
            ExecuteWithTempAppSettings("{\"Jwt\":{\"Secret\":\"abc123\"}}", () =>
            {
                var result = _service.GetAppSetting("Jwt:Secret");

                Assert.True(result.Success);
                Assert.Equal("abc123", result.Data);
                Assert.Equal("Key 'Jwt:Secret' retrieved successfully.", result.Message);
            });
        }

        [Fact]
        public void GetAppSetting_WhenKeyDoesNotExist_ReturnsFailure()
        {
            ExecuteWithTempAppSettings("{\"Jwt\":{\"Secret\":\"abc123\"}}", () =>
            {
                var result = _service.GetAppSetting("Jwt:Audience");

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Key 'Jwt:Audience' not found.", result.Message);
            });
        }

        [Fact]
        public void GetAppSetting_WhenSectionIsInvalid_ReturnsFailure()
        {
            ExecuteWithTempAppSettings("{\"Jwt\":\"abc123\"}", () =>
            {
                var result = _service.GetAppSetting("Jwt:Secret");

                Assert.False(result.Success);
                Assert.Null(result.Data);
                Assert.Equal("Section 'Jwt' not found or is not an object.", result.Message);
            });
        }

        [Fact]
        public void GetAppSetting_WhenJsonIsInvalid_ReturnsFailure()
        {
            ExecuteWithTempAppSettings("{ invalid json }", () =>
            {
                var result = _service.GetAppSetting("Jwt:Secret");

                Assert.False(result.Success);
                Assert.Contains("Error reading appsettings.json:", result.Message);
                Assert.NotEmpty(result.Errors);
            });
        }

        [Fact]
        public void UpdateAppSetting_WhenKeyExists_UpdatesValue()
        {
            ExecuteWithTempAppSettings("{\"Jwt\":{\"Secret\":\"abc123\"}}", () =>
            {
                var result = _service.UpdateAppSetting("Jwt:Secret", "updated-secret");
                var fileContents = File.ReadAllText("appsettings.json");

                Assert.True(result.Success);
                Assert.True(result.Data);
                Assert.Equal("Key 'Jwt:Secret' updated to 'updated-secret' successfully.", result.Message);
                Assert.Contains("updated-secret", fileContents);
            });
        }

        [Fact]
        public void UpdateAppSetting_WhenSectionIsInvalid_ReturnsFailure()
        {
            ExecuteWithTempAppSettings("{\"Jwt\":\"abc123\"}", () =>
            {
                var result = _service.UpdateAppSetting("Jwt:Secret", "updated-secret");

                Assert.False(result.Success);
                Assert.False(result.Data);
                Assert.Equal("Section 'Jwt' not found or is not an object.", result.Message);
            });
        }

        [Fact]
        public void UpdateAppSetting_WhenFileDoesNotExist_ReturnsFailure()
        {
            lock (SyncLock)
            {
                var originalDirectory = Directory.GetCurrentDirectory();
                var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempDirectory);
                Directory.SetCurrentDirectory(tempDirectory);

                try
                {
                    var result = _service.UpdateAppSetting("Jwt:Secret", "updated-secret");

                    Assert.False(result.Success);
                    Assert.False(result.Data);
                    Assert.Contains("Error updating appsettings.json:", result.Message);
                    Assert.NotEmpty(result.Errors);
                }
                finally
                {
                    Directory.SetCurrentDirectory(originalDirectory);
                    Directory.Delete(tempDirectory, true);
                }
            }
        }

        [Fact]
        public void UpdateAppSetting_WhenJsonIsInvalid_ReturnsFailure()
        {
            ExecuteWithTempAppSettings("{ invalid json }", () =>
            {
                var result = _service.UpdateAppSetting("Jwt:Secret", "updated-secret");

                Assert.False(result.Success);
                Assert.False(result.Data);
                Assert.Contains("Error updating appsettings.json:", result.Message);
                Assert.NotEmpty(result.Errors);
            });
        }

        private static void ExecuteWithTempAppSettings(string appSettingsContent, Action assertion)
        {
            lock (SyncLock)
            {
                var originalDirectory = Directory.GetCurrentDirectory();
                var tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
                Directory.CreateDirectory(tempDirectory);
                File.WriteAllText(Path.Combine(tempDirectory, "appsettings.json"), appSettingsContent);
                Directory.SetCurrentDirectory(tempDirectory);

                try
                {
                    assertion();
                }
                finally
                {
                    Directory.SetCurrentDirectory(originalDirectory);
                    Directory.Delete(tempDirectory, true);
                }
            }
        }
    }
}
