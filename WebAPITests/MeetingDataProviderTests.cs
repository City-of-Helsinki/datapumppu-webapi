using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using WebAPI.Data;
using WebAPI.StorageClient;
using WebAPI.StorageClient.DTOs;
using Xunit;

namespace WebAPITests
{
    public class MeetingDataProviderTests
    {
        [Fact]
        public async Task GetMeeting_ReturnsMeetingFromApiClient_OnCacheMiss()
        {
            // Arrange
            var year = "2026";
            var sequenceNumber = "12";
            var language = "fi";
            var expectedMeeting = new StorageMeetingDTO { MeetingID = "test-id-123", Name = "Test Meeting" };

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
                .Setup(x => x.RequestMeeting(year, sequenceNumber, language))
                .ReturnsAsync(expectedMeeting);

            var provider = new MeetingDataProvider(serviceProviderMock.Object);

            // Act
            var result = await provider.GetMeeting(year, sequenceNumber, language);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test-id-123", result.MeetingID);
            Assert.Equal("Test Meeting", result.Name);

            storageApiClientMock.Verify(x => x.RequestMeeting(year, sequenceNumber, language), Times.Once);
        }

        [Fact]
        public async Task GetMeeting_ReturnsCachedMeeting_OnCacheHit()
        {
            // Arrange
            var year = "2026";
            var sequenceNumber = "12";
            var language = "fi";
            var expectedMeeting = new StorageMeetingDTO { MeetingID = "test-id-123", Name = "Test Meeting" };

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
                .Setup(x => x.RequestMeeting(year, sequenceNumber, language))
                .ReturnsAsync(expectedMeeting);

            var provider = new MeetingDataProvider(serviceProviderMock.Object);

            // Act - Request first time (Cache Miss)
            var result1 = await provider.GetMeeting(year, sequenceNumber, language);

            // Act - Request second time (Cache Hit)
            var result2 = await provider.GetMeeting(year, sequenceNumber, language);

            // Assert
            Assert.Same(result1, result2);
            storageApiClientMock.Verify(x => x.RequestMeeting(year, sequenceNumber, language), Times.Once);
        }

        [Fact]
        public async Task GetMeeting_FetchesFromApiClient_OnCacheExpiry()
        {
            // Arrange
            var year = "2026";
            var sequenceNumber = "12";
            var language = "fi";
            var expectedMeeting1 = new StorageMeetingDTO { MeetingID = "test-id-1", Name = "Meeting 1" };
            var expectedMeeting2 = new StorageMeetingDTO { MeetingID = "test-id-2", Name = "Meeting 2" };

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
                .SetupSequence(x => x.RequestMeeting(year, sequenceNumber, language))
                .ReturnsAsync(expectedMeeting1)
                .ReturnsAsync(expectedMeeting2);

            var provider = new MeetingDataProvider(serviceProviderMock.Object);

            // Act - First request (Cache Miss)
            var result1 = await provider.GetMeeting(year, sequenceNumber, language);
            Assert.Equal("test-id-1", result1?.MeetingID);

            // Use reflection to modify the cache entry timestamp to trigger expiration
            var field = typeof(MeetingDataProvider).GetField("_dataCache", BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.NotNull(field);
            var dataCache = (ConcurrentDictionary<string, MeetingDataCache>)field.GetValue(provider)!;
            
            var cacheKey = $"{year}-{sequenceNumber}-{language}";
            Assert.True(dataCache.TryGetValue(cacheKey, out var cacheItem));
            Assert.NotNull(cacheItem);
            
            // Set timestamp to 6 minutes ago (cache limit is 5 minutes)
            cacheItem.Timestamp = DateTime.UtcNow.AddMinutes(-6);

            // Act - Second request (Cache Expired, should call API again)
            var result2 = await provider.GetMeeting(year, sequenceNumber, language);
            Assert.Equal("test-id-2", result2?.MeetingID);

            storageApiClientMock.Verify(x => x.RequestMeeting(year, sequenceNumber, language), Times.Exactly(2));
        }

        [Fact]
        public async Task GetMeeting_ThrowsInvalidOperationException_WhenApiClientResolutionFails()
        {
            // Arrange
            var year = "2026";
            var sequenceNumber = "12";
            var language = "fi";

            var serviceProviderMock = new Mock<IServiceProvider>();
            var serviceScopeFactoryMock = new Mock<IServiceScopeFactory>();
            var serviceScopeMock = new Mock<IServiceScope>();
            var scopeServiceProviderMock = new Mock<IServiceProvider>();

            serviceProviderMock
                .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
                .Returns(serviceScopeFactoryMock.Object);

            serviceScopeFactoryMock
                .Setup(x => x.CreateScope())
                .Returns(serviceScopeMock.Object);

            serviceScopeMock
                .Setup(x => x.ServiceProvider)
                .Returns(scopeServiceProviderMock.Object);

            // Mock returns null for IStorageApiClient
            scopeServiceProviderMock
                .Setup(x => x.GetService(typeof(IStorageApiClient)))
                .Returns((IStorageApiClient?)null);

            var provider = new MeetingDataProvider(serviceProviderMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                provider.GetMeeting(year, sequenceNumber, language)
            );
        }
    }
}
