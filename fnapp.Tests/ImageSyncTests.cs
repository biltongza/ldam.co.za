using System;
using Xunit;
using Moq;
using ldam.co.za.fnapp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ldam.co.za.fnapp;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Threading.Tasks;
using ldam.co.za.fnapp.Models;

namespace fnapp.Tests
{
    public class ImageSyncTests
    {
        const string ManifestName = "manifest.json";
        private readonly Mock<ILightroomService> mockLightroomService = new();
        private readonly Mock<IConfiguration> mockConfiguration = new();
        private readonly Mock<IStorageService> mockStorageService = new();
        private readonly Mock<ILogger<SyncService>> mockLogger = new();
        private readonly SyncService syncService;
        public ImageSyncTests()
        {
            mockConfiguration.Setup(x => x[Constants.Configuration.Adobe.AlbumIds]).Returns("testalbum1");
            mockConfiguration.Setup(x => x[Constants.Configuration.Adobe.SizesToSync]).Returns("2048");
            syncService = new SyncService(
                mockLightroomService.Object,
                mockConfiguration.Object,
                mockStorageService.Object,
                mockLogger.Object
            );
        }

        [Fact]
        public async Task ShouldCreateManifestIfItDoesNotExist()
        {
            mockLightroomService
                .Setup(x => x.GetAlbums())
                .Returns(new Dictionary<string, string>().ToAsyncEnumerable());

            mockLightroomService
                .Setup(x => x.GetImageList(It.IsAny<string>()))
                .Returns(new List<ImageInfo>().ToAsyncEnumerable());
            
            mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(Stream.Null));

            await syncService.SyncImages();

            mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>()), Times.Once);
        }
    }
}
