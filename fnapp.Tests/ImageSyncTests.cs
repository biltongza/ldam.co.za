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
using System.Text.Json;

namespace ldam.co.za.fnapp.Tests
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

        [Fact]
        public async Task ShouldNotModifyManifestIfNoChangesToAlbum()
        {
            var manifestImages = new Dictionary<string, ImageMetadata>
            {
                { "image1",  new ImageMetadata { Id = "image1" } },
            };

            var adobeImages = new List<ImageInfo>
            {
                new ImageInfo { AssetId = "image1" }
            };

            var mockManifest = new Manifest
            {
                LastModified = new DateTime(2021, 8, 8),
                Albums = new Dictionary<string, Album>
                {
                    { "testalbum1", new Album { Id = "testalbum1", Images = manifestImages}},
                }
            };

            Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest));

            mockLightroomService
                .Setup(x => x.GetAlbums())
                .Returns(new Dictionary<string, string>() { { "testalbum1", "Test album 1" } }.ToAsyncEnumerable());

            mockLightroomService
                .Setup(x => x.GetImageList(It.IsAny<string>()))
                .Returns(adobeImages.ToAsyncEnumerable());

            mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(serialisedManifest));

            await syncService.SyncImages();

            mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>()), Times.Never);
        }

        [Fact]
        public async Task ShouldUpdateManifestWhenNewAdobeImagePresent()
        {
            var manifestImages = new Dictionary<string, ImageMetadata>
            {
                { "image1",  new ImageMetadata { Id = "image1" } },
            };

            var adobeImages = new List<ImageInfo>
            {
                new ImageInfo { AssetId = "image1" },
                new ImageInfo { AssetId = "image2 "},
            };

            var mockManifest = new Manifest
            {
                LastModified = new DateTime(2021, 8, 8),
                Albums = new Dictionary<string, Album>
                {
                    { "testalbum1", new Album { Id = "testalbum1", Images = manifestImages}},
                }
            };

            Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest));

            mockLightroomService
                .Setup(x => x.GetAlbums())
                .Returns(new Dictionary<string, string>() { { "testalbum1", "Test album 1" } }.ToAsyncEnumerable());

            mockLightroomService
                .Setup(x => x.GetImageList(It.IsAny<string>()))
                .Returns(adobeImages.ToAsyncEnumerable());

            mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(serialisedManifest));

            await syncService.SyncImages();

            mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>()), Times.Once);
        }
    }
}
