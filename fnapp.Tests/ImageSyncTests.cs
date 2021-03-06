using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ldam.co.za.lib;
using ldam.co.za.fnapp.Services;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using Microsoft.Extensions.Options;

public class ImageSyncTests
{
    const string ManifestName = "manifest.json";
    private readonly Mock<ILightroomService> mockLightroomService = new();
    private readonly Mock<IOptionsSnapshot<FunctionAppLightroomOptions>> mockOptions = new();
    private readonly Mock<IStorageService> mockStorageService = new();
    private readonly Mock<ILogger<SyncService>> mockLogger = new();
    private readonly Mock<IMetadataService> mockMetadataService = new();
    private readonly Mock<ICdnService> mockCdnService = new();
    private readonly SyncService syncService;
    public ImageSyncTests()
    {
        mockOptions.SetupGet(x => x.Value).Returns(new FunctionAppLightroomOptions
        {
            AlbumIds = "testalbum1",
            SizesToSync = "2048"
        });
        mockMetadataService.Setup(x => x.MapAdobeMetadataToManifestMetadata(It.IsAny<ImageInfo>())).Returns(new ImageMetadata());
        syncService = new SyncService(
            mockLightroomService.Object,
            mockOptions.Object,
            mockStorageService.Object,
            mockLogger.Object,
            mockMetadataService.Object,
            mockCdnService.Object
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

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
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
                new ImageInfo { AssetId = "image1", Width = 1, Height = 1 }
            };

        var mockManifest = new Manifest
        {
            LastModified = new DateTime(2021, 8, 8),
            Albums = new Dictionary<string, Album>
                {
                    { "testalbum1", new Album { Id = "testalbum1", Images = manifestImages}},
                }
        };

        Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest, typeof(Manifest), ManifestSerializerContext.Default));

        mockLightroomService
            .Setup(x => x.GetAlbums())
            .Returns(new Dictionary<string, string>() { { "testalbum1", "Test album 1" } }.ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList(It.IsAny<string>()))
            .Returns(adobeImages.ToAsyncEnumerable());

        mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(serialisedManifest));

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>()), Times.Never);
        mockCdnService.Verify(x => x.ClearCache(It.IsAny<string>()), Times.Never);
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
                new ImageInfo { AssetId = "image1", Width = 1, Height = 1, ShutterSpeed = new int[2] {1,1}, FNumber = new int[2] {1,1}, CaptureDate = default },
                new ImageInfo { AssetId = "image2", Width = 1, Height = 1, ShutterSpeed = new int[2] {1,1}, FNumber = new int[2] {1,1}, CaptureDate = default },
            };

        var mockManifest = new Manifest
        {
            LastModified = new DateTime(2021, 8, 8),
            Albums = new Dictionary<string, Album>
                {
                    { "testalbum1", new Album { Id = "testalbum1", Images = manifestImages}},
                }
        };

        Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest, typeof(Manifest), ManifestSerializerContext.Default));

        mockLightroomService
            .Setup(x => x.GetAlbums())
            .Returns(new Dictionary<string, string>() { { "testalbum1", "Test album 1" } }.ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList(It.IsAny<string>()))
            .Returns(adobeImages.ToAsyncEnumerable());

        mockLightroomService.Setup(x => x.GetImageStream(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult<Stream>(new MemoryStream()));

        mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(serialisedManifest));

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>()), Times.Once);
        mockCdnService.Verify(x => x.ClearCache(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task ShouldRemoveImagesPresentInManfestButNotInAlbum()
    {
        var manifestImages = new Dictionary<string, ImageMetadata>
            {
                { "image1",  new ImageMetadata { Id = "image1" } },
            };

        var adobeImages = new List<ImageInfo>();

        var mockManifest = new Manifest
        {
            LastModified = new DateTime(2021, 8, 8),
            Albums = new Dictionary<string, Album>
                {
                    { "testalbum1", new Album { Id = "testalbum1", Images = manifestImages}},
                }
        };

        Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest, typeof(Manifest), ManifestSerializerContext.Default));

        mockLightroomService
            .Setup(x => x.GetAlbums())
            .Returns(new Dictionary<string, string>() { { "testalbum1", "Test album 1" } }.ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList(It.IsAny<string>()))
            .Returns(adobeImages.ToAsyncEnumerable());

        mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(serialisedManifest));

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.DeleteBlobsStartingWith("image1"), Times.Once);
        mockCdnService.Verify(x => x.ClearCache(It.IsAny<string>()), Times.Once);
    }
}

