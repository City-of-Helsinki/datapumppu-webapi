using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using WebAPI.Controllers;
using Xunit;

namespace WebAPITests
{
    public class WebComponentsControllerTests : IDisposable
    {
        private const string DummyFileDirectory = "./ScriptFiles/components";
        private const string DummyFilePath = "./ScriptFiles/components/meeting.js";
        private readonly string _originalFileContent = "API: #--API_URL--#, YEAR: #--MEETING_YEAR--#, SEQ: #--MEETING_SEQUENCE_NUM--#, LANG: #--LANGUAGE--#";

        public WebComponentsControllerTests()
        {
            // Ensure the directory and dummy file exist for tests
            Directory.CreateDirectory(DummyFileDirectory);
            File.WriteAllText(DummyFilePath, _originalFileContent);
        }

        public void Dispose()
        {
            // Clean up the dummy file and directory after tests
            if (File.Exists(DummyFilePath))
            {
                File.Delete(DummyFilePath);
            }
            if (Directory.Exists(DummyFileDirectory) && Directory.GetFiles(DummyFileDirectory).Length == 0)
            {
                Directory.Delete(DummyFileDirectory);
            }
        }

        [Fact]
        public async Task GetMeeting_ReturnsFileResultWithReplacedPlaceholders_OnValidInput()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["API_URL"]).Returns("http://localhost:8081");

            var loggerMock = new Mock<ILogger<WebComponentsController>>();

            var controller = new WebComponentsController(configMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetMeeting("2026", "42", "fi");

            // Assert
            var fileResult = Assert.IsType<FileContentResult>(result);
            Assert.Equal("application/javascript", fileResult.ContentType);

            var fileContent = Encoding.UTF8.GetString(fileResult.FileContents);
            Assert.Equal("API: http://localhost:8081, YEAR: 2026, SEQ: 42, LANG: fi", fileContent);
        }

        [Fact]
        public async Task GetMeeting_Returns500_WhenApiUrlConfigIsMissing()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["API_URL"]).Returns((string?)null); // Missing API_URL

            var loggerMock = new Mock<ILogger<WebComponentsController>>();

            var controller = new WebComponentsController(configMock.Object, loggerMock.Object);

            // Act
            var result = await controller.GetMeeting("2026", "42", "fi");

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetMeeting_Returns500_WhenYearIsInvalid()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["API_URL"]).Returns("http://localhost:8081");

            var loggerMock = new Mock<ILogger<WebComponentsController>>();

            var controller = new WebComponentsController(configMock.Object, loggerMock.Object);

            // Act - Year is not a valid integer
            var result = await controller.GetMeeting("invalid-year", "42", "fi");

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetMeeting_Returns500_WhenSequenceNumberIsInvalid()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["API_URL"]).Returns("http://localhost:8081");

            var loggerMock = new Mock<ILogger<WebComponentsController>>();

            var controller = new WebComponentsController(configMock.Object, loggerMock.Object);

            // Act - Sequence number is not a valid integer
            var result = await controller.GetMeeting("2026", "invalid-seq", "fi");

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task GetMeeting_Returns500_WhenLanguageIsInvalid()
        {
            // Arrange
            var configMock = new Mock<IConfiguration>();
            configMock.Setup(c => c["API_URL"]).Returns("http://localhost:8081");

            var loggerMock = new Mock<ILogger<WebComponentsController>>();

            var controller = new WebComponentsController(configMock.Object, loggerMock.Object);

            // Act - Language is not supported (e.g., French "fr" or Spanish "es")
            var result = await controller.GetMeeting("2026", "42", "fr");

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal(StatusCodes.Status500InternalServerError, statusCodeResult.StatusCode);
        }
    }
}
