using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using WebAPI.StorageClient;
using Xunit;

namespace WebAPITests
{
    public class StorageApiClientTests
    {
        [Fact]
        public async Task CheckLogin_ReturnsTrue_WhenResponseIsSuccess()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<StorageApiClient>>();
            var storageConnectionMock = new Mock<IStorageConnection>();

            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
               .Protected()
               // Setup the PROTECTED method to mock
               .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
               )
               // prepare the expected response of the mocked http call
               .ReturnsAsync(new HttpResponseMessage()
               {
                   StatusCode = HttpStatusCode.OK,
                   Content = new StringContent("[{'id':1,'value':'1'}]"),
               })
               .Verifiable();

            // use real http client with mocked handler here
            var httpClient = new HttpClient(handlerMock.Object)
            {
                BaseAddress = new System.Uri("http://test.com/")
            };

            storageConnectionMock.Setup(x => x.CreateConnection()).Returns(httpClient);

            var apiClient = new StorageApiClient(loggerMock.Object, storageConnectionMock.Object);

            // Act
            var result = await apiClient.CheckLogin("testuser", "testpass");

            // Assert
            Assert.True(result);
            
            // Verify that it sent a POST request
            handlerMock.Protected().Verify(
               "SendAsync",
               Times.Exactly(1),
               ItExpr.Is<HttpRequestMessage>(req =>
                  req.Method == HttpMethod.Post && req.RequestUri.ToString() == "http://test.com/api/auth/validate"
               ),
               ItExpr.IsAny<CancellationToken>()
            );
        }
    }
}