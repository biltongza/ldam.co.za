using System.Text.Json;
using ldam.co.za.fnapp.Services;
using ldam.co.za.lib;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class ImageSyncTests
{
    const string ManifestName = "manifest.json";
    private readonly Mock<ILightroomService> mockLightroomService = new();
    private readonly Mock<IOptionsSnapshot<FunctionAppLightroomOptions>> mockOptions = new();
    private readonly Mock<IStorageService> mockStorageService = new();
    private readonly Mock<ILogger<SyncService>> mockLogger = new();
    private readonly Mock<IMetadataService> mockMetadataService = new();
    private readonly Mock<ICdnService> mockCdnService = new();
    private readonly Mock<IWebPEncoderService> mockWebpEncoderService = new();
    private readonly SyncService syncService;
    public ImageSyncTests()
    {
        mockOptions.SetupGet(x => x.Value).Returns(new FunctionAppLightroomOptions
        {
            PortfolioAlbumId = "testalbum1",
            SizesToSync = "2048",
            CollectionsContainerAlbumId = "testCollection1",
            BaseUrl = new Uri("/"),
            ClientId = string.Empty,
            RefreshTokenWindowMinutes = string.Empty,
        });
        mockMetadataService.Setup(x => x.MapAdobeMetadataToManifestMetadata(It.IsAny<ImageInfo>())).Returns(EmptyImageMetadata());
        syncService = new SyncService(
            mockLightroomService.Object,
            mockOptions.Object,
            mockStorageService.Object,
            mockLogger.Object,
            mockMetadataService.Object,
            mockCdnService.Object,
            mockWebpEncoderService.Object
        );
    }

    static ImageMetadata EmptyImageMetadata(string? id = null) => new ImageMetadata
    {
        Id = id ?? string.Empty,
        AspectRatio = string.Empty,
        CameraModel = string.Empty,
        Caption = string.Empty,
        CaptureDate = default,
        FNumber = string.Empty,
        FocalLength = string.Empty,
        Hrefs = new Dictionary<string, string>(),
        ISO = string.Empty,
        LastModified = default,
        Lens = string.Empty,
        ShutterSpeed = string.Empty,
        Title = string.Empty,
    };

    [Fact]
    public async Task ShouldCreateManifestIfItDoesNotExist()
    {
        mockLightroomService
            .Setup(x => x.GetAlbums(It.IsAny<CancellationToken>()))
            .Returns(new List<AlbumInfo>().ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new List<ImageInfo>().ToAsyncEnumerable());

        mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(Stream.Null));

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldNotModifyManifestIfNoChangesToAlbum()
    {
        var manifestImages = new Dictionary<string, ImageMetadata>
            {
                { "image1",  EmptyImageMetadata("image1") },
            };

        var adobeImages = new List<ImageInfo>
            {
                new ImageInfo { AssetId = "image1", Width = 1, Height = 1 }
            };

        var mockManifest = new Manifest
        {
            LastModified = new DateTime(2021, 8, 8),
            Albums = new List<Album>
                {
                    new Album { Id = "testalbum1", Title = "testalbum1", Images = manifestImages},
                }
        };

        Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest, typeof(Manifest), ManifestSerializerContext.Default));

        mockLightroomService
            .Setup(x => x.GetAlbums(It.IsAny<CancellationToken>()))
            .Returns(
                new List<AlbumInfo>
                {
                    new AlbumInfo
                    {
                        Id = "testalbum1",
                        Title = "Test album 1"
                    }
                }.ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList("testalbum1", It.IsAny<CancellationToken>()))
            .Returns(adobeImages.ToAsyncEnumerable());

        mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(serialisedManifest));

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
        mockCdnService.Verify(x => x.ClearCache(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task ShouldUpdateManifestWhenNewAdobeImagePresentInCollection()
    {
        var manifestImages = new Dictionary<string, ImageMetadata>
            {
                { "image1",  EmptyImageMetadata("image1") },
            };

        var adobeImages = new List<ImageInfo>
            {
                new ImageInfo { AssetId = "image1", Width = 1, Height = 1, ShutterSpeed = new int[2] {1,1}, FNumber = new int[2] {1,1}, CaptureDate = default },
                new ImageInfo { AssetId = "image2", Width = 1, Height = 1, ShutterSpeed = new int[2] {1,1}, FNumber = new int[2] {1,1}, CaptureDate = default },
            };

        var mockManifest = new Manifest
        {
            LastModified = new DateTime(2021, 8, 8),
            Albums = new List<Album>
                {
                    new Album { Id = "testalbum1", Title = "testalbum1", Images = manifestImages},
                }
        };

        Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest, typeof(Manifest), ManifestSerializerContext.Default));

        mockLightroomService
            .Setup(x => x.GetAlbums(It.IsAny<CancellationToken>()))
            .Returns(
                new List<AlbumInfo>
                {
                    new AlbumInfo
                    {
                        Id = "testalbum1",
                        Title = "Test album 1"
                    }
                }.ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(adobeImages.ToAsyncEnumerable());

        mockLightroomService.Setup(x => x.GetImageStream(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<Stream>(new MemoryStream()));

        mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(serialisedManifest));
        mockMetadataService
            .Setup(x => x.SetImageMetadata(It.IsAny<Stream>(), It.IsAny<ImageInfo>()))
            .Returns((Stream value, ImageInfo _) => Task.FromResult(value));

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        mockCdnService.Verify(x => x.ClearCache(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldRemoveImagesPresentInManfestButNotInAlbum()
    {
        var manifestImages = new Dictionary<string, ImageMetadata>
            {
                { "image1",  EmptyImageMetadata("image1") },
            };

        var adobeImages = new List<ImageInfo>();

        var mockManifest = new Manifest
        {
            LastModified = new DateTime(2021, 8, 8),
            Albums = new List<Album>
                {
                    new Album { Id = "testalbum1", Title = "testalbum1", Images = manifestImages},
                }
        };

        Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest, typeof(Manifest), ManifestSerializerContext.Default));

        mockLightroomService
            .Setup(x => x.GetAlbums(It.IsAny<CancellationToken>()))
            .Returns(
                new List<AlbumInfo>
                {
                    new AlbumInfo
                    {
                        Id = "testalbum1",
                        Title = "Test album 1"
                    }
                }.ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(adobeImages.ToAsyncEnumerable());

        mockStorageService.Setup(x => x.Get(ManifestName)).Returns(Task.FromResult(serialisedManifest));

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.DeleteBlobsStartingWith("image1", It.IsAny<CancellationToken>()), Times.Once);
        mockCdnService.Verify(x => x.ClearCache(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ShouldMarkPortfolioAlbum()
    {
        mockLightroomService
            .Setup(x => x.GetAlbums(It.IsAny<CancellationToken>()))
            .Returns(new List<AlbumInfo>
                {
                    new AlbumInfo
                    {
                        Id = "testalbum1",
                        Title = "testalbum1",
                    }
                }.ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(new List<ImageInfo>().ToAsyncEnumerable());

        var mockManifest = new Manifest();

        Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest, typeof(Manifest), ManifestSerializerContext.Default));

        mockStorageService
            .Setup(x => x.Get(ManifestName))
            .Returns(Task.FromResult(serialisedManifest));

        Manifest? manifest = null;

        mockStorageService
            .Setup(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback<string, Stream, string, CancellationToken>((_, stream, _, _) =>
            {
                manifest = JsonSerializer.Deserialize(stream, typeof(Manifest), ManifestSerializerContext.Default) as Manifest;
            });

        await syncService.Synchronize(false);
        mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(manifest);
        Assert.NotEmpty(manifest.Albums);
        Assert.True(manifest.Albums.Single(x => x.Id == "testalbum1").IsPortfolio);
    }

    [Fact]
    public async Task RemovesCollectionInManifestNotPresentInAdobe()
    {
        var manifestImages = new Dictionary<string, ImageMetadata>
            {
                { "image1", EmptyImageMetadata("image1") },
            };

        var manifestImagesToRemove = new Dictionary<string, ImageMetadata>
            {
                { "image2", EmptyImageMetadata("image2")},
            };

        var adobeImages = new List<ImageInfo>
            {
                new ImageInfo { AssetId = "image1", Width = 1, Height = 1, ShutterSpeed = [1,1], FNumber = [1,1], CaptureDate = default },
            };

        var mockManifest = new Manifest
        {
            LastModified = new DateTime(2021, 8, 8),
            Albums = new List<Album>
                {
                    new Album { Id = "testalbum1", Title = "testalbum1", Images = manifestImages },
                    new Album { Id = "testalbum2", Title = "testalbum2", Images = manifestImagesToRemove },
                }
        };

        Stream serialisedManifest = new MemoryStream(JsonSerializer.SerializeToUtf8Bytes(mockManifest, typeof(Manifest), ManifestSerializerContext.Default));

        mockLightroomService
            .Setup(x => x.GetAlbums(It.IsAny<CancellationToken>()))
            .Returns(
                new List<AlbumInfo>
                {
                    new AlbumInfo
                    {
                        Id = "testalbum1",
                        Title = "Test album 1"
                    }
                }.ToAsyncEnumerable());

        mockLightroomService
            .Setup(x => x.GetImageList(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(adobeImages.ToAsyncEnumerable());

        mockLightroomService.Setup(x => x.GetImageStream(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult<Stream>(new MemoryStream()));

        mockStorageService
            .Setup(x => x.Get(ManifestName))
            .Returns(Task.FromResult(serialisedManifest));
        mockMetadataService
            .Setup(x => x.SetImageMetadata(It.IsAny<Stream>(), It.IsAny<ImageInfo>()))
            .Returns((Stream value, ImageInfo _) => Task.FromResult(value));

        mockStorageService
            .Setup(x => x.DeleteBlobsStartingWith(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        await syncService.Synchronize(false);

        mockStorageService.Verify(x => x.Store(ManifestName, It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
        mockStorageService.Verify(x => x.DeleteBlobsStartingWith("image2", It.IsAny<CancellationToken>()), Times.Once);
        mockCdnService.Verify(x => x.ClearCache(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}

