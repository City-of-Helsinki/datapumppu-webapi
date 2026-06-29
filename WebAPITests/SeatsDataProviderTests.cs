using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebAPI.Controllers.DTOs;
using WebAPI.Data;
using WebAPI.StorageClient;
using Xunit;

namespace WebAPITests
{
    public class SeatsDataProviderTests
    {
        [Fact]
        public async Task GetSeats_ReturnsSeatsFromApiClient_OnCacheMiss()
        {
            // Arrange
            var meetingId = "meeting-1";
            var caseNumber = "5";
            
            var expectedSeats = new List<WebApiSeatsDTO>
            {
                new WebApiSeatsDTO
                {
                    VotingNumber = 0,
                    Seats = new List<WebApiSeatDTO>
                    {
                        new WebApiSeatDTO { SeatId = "1", Person = "John Doe" }
                    }
                }
            };

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            var scopeServiceProviderMock = new Mock<IServiceProvider>();
            var storageApiClientMock = new Mock<IStorageApiClient>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactoryMock.Object);

            serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            serviceScopeMock
                .Setup(x => x.ServiceProvider)
                .Returns(scopeServiceProviderMock.Object);

            scopeServiceProviderMock
                .Setup(x => x.GetService(typeof(IStorageApiClient)))
                .Returns(storageApiClientMock.Object);

            storageApiClientMock
                .Setup(x => x.RequestSeats(meetingId, caseNumber))
                .ReturnsAsync(expectedSeats);

            var provider = new SeatsDataProvider(serviceProviderMock.Object);

            // Act
            var result = await provider.GetSeats(meetingId, caseNumber);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(0, result[0].VotingNumber);
            Assert.Single(result[0].Seats);
            Assert.Equal("John Doe", result[0].Seats[0].Person);

            storageApiClientMock.Verify(x => x.RequestSeats(meetingId, caseNumber), Times.Once);
        }

        [Fact]
        public async Task GetSeats_ReturnsCachedSeats_OnCacheHit()
        {
            // Arrange
            var meetingId = "meeting-1";
            var caseNumber = "5";

            var expectedSeats = new List<WebApiSeatsDTO>
            {
                new WebApiSeatsDTO
                {
                    VotingNumber = 1,
                    Seats = new List<WebApiSeatDTO>
                    {
                        new WebApiSeatDTO { SeatId = "12", Person = "Jane Smith" }
                    }
                }
            };

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            var scopeServiceProviderMock = new Mock<IServiceProvider>();
            var storageApiClientMock = new Mock<IStorageApiClient>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactoryMock.Object);

            serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            serviceScopeMock
                .Setup(x => x.ServiceProvider)
                .Returns(scopeServiceProviderMock.Object);

            scopeServiceProviderMock
                .Setup(x => x.GetService(typeof(IStorageApiClient)))
                .Returns(storageApiClientMock.Object);

            storageApiClientMock
                .Setup(x => x.RequestSeats(meetingId, caseNumber))
                .ReturnsAsync(expectedSeats);

            var provider = new SeatsDataProvider(serviceProviderMock.Object);

            // Act - Request first time (Cache Miss)
            var result1 = await provider.GetSeats(meetingId, caseNumber);

            // Act - Request second time (Cache Hit)
            var result2 = await provider.GetSeats(meetingId, caseNumber);

            // Assert
            Assert.Same(result1, result2);
            storageApiClientMock.Verify(x => x.RequestSeats(meetingId, caseNumber), Times.Once);
        }
    }
}
