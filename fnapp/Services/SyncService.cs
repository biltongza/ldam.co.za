using System.Text.Json;
using ldam.co.za.lib;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ldam.co.za.fnapp.Services;

public class SyncService
{
    private readonly ILightroomService lightroomService;
    private readonly IOptionsSnapshot<FunctionAppLightroomOptions> options;
    private readonly IStorageService storageService;
    private readonly ILogger logger;
    private readonly IMetadataService metadataService;
    private readonly ICdnService cdnService;

    public const string JpgMimeType = "image/jpeg";
    public const string JsonMimeType = "application/json";
    private const string ManifestName = "manifest.json";

    public SyncService(
        ILightroomService lightroomService,
        IOptionsSnapshot<FunctionAppLightroomOptions> options,
        IStorageService storageService,
        ILogger<SyncService> logger,
        IMetadataService metadataService,
        ICdnService cdnService)
    {
        this.lightroomService = lightroomService;
        this.options = options;
        this.storageService = storageService;
        this.logger = logger;
        this.metadataService = metadataService;
        this.cdnService = cdnService;
    }

    public async Task Synchronize(bool force, CancellationToken cancellationToken = default)
    {
        if (force)
        {
            logger.LogInformation("Forcing full refresh");
        }
        var portfolioAlbumId = options.Value.PortfolioAlbumId;
        var collectionsContainerAlbumId = options.Value.CollectionsContainerAlbumId;
        var sizesToSync = options.Value.SizesToSync.Split(',');

        (Manifest manifest, bool syncManifest) = await GetOrCreateManifest(ManifestName, force);

        if (manifest == null)
        {
            throw new InvalidOperationException("Manifest is null! Is the json in storage valid?");
        }

        var albumsToSync = await lightroomService.GetAlbums(cancellationToken)
            .Where(album => portfolioAlbumId.Equals(album.Id) || collectionsContainerAlbumId.Equals(album.ParentId))
            .ToListAsync(cancellationToken);

        var syncTasks = albumsToSync
            .Select(async collection => await CreateOrUpdateCollection(manifest, collection, sizesToSync, portfolioAlbumId, cancellationToken))
            .ToList();

        syncManifest |= (await Task.WhenAll(syncTasks)).Any(x => x);

        if (syncManifest)
        {
            manifest.LastModified = DateTime.Now;
            using var serializedStream = new MemoryStream();
            await JsonSerializer.SerializeAsync(serializedStream, manifest, ManifestSerializerContext.Default.Manifest, cancellationToken);
            serializedStream.Seek(0, SeekOrigin.Begin);
            await storageService.Store(ManifestName, serializedStream, JsonMimeType, cancellationToken);
            logger.LogInformation("Manifest {manifestName} updated", ManifestName);
            logger.LogInformation("Purging CDN cache");
            await cdnService.ClearCache("/*", cancellationToken);
            logger.LogInformation("CDN cache purged");
        }
    }

    private async Task<(Manifest, bool)> GetOrCreateManifest(string manifestName, bool forceCreate)
    {
        Manifest manifest;
        bool didCreate = false;
        using var manifestStream = await storageService.Get(manifestName);
        if (manifestStream == Stream.Null || forceCreate)
        {
            manifest = new Manifest();
            didCreate = true;
        }
        else
        {
            manifest = await JsonSerializer.DeserializeAsync(manifestStream, typeof(Manifest), ManifestSerializerContext.Default) as Manifest;
        }

        return (manifest, didCreate);
    }

    private async Task<bool> CreateOrUpdateCollection(Manifest manifest, AlbumInfo collection, string[] sizesToSync, string portfolioAlbumId, CancellationToken cancellationToken = default)
    {
        var collectionAdded = false;
        Album manifestCollection;
        lock (manifest.Albums)
        {
            manifestCollection = manifest.Albums.FirstOrDefault(x => collection.Id.Equals(x.Id));

            if (manifestCollection == null)
            {
                logger.LogInformation("Collection {albumId} not present in manifest, syncing", collection);
                manifestCollection = new Album
                {
                    Id = collection.Id,
                    Title = collection.Title,
                    IsPortfolio = portfolioAlbumId.Equals(collection.Id),
                    Created = collection.Created,
                    Updated = collection.Updated,
                };

                manifest.Albums.Add(manifestCollection);
                collectionAdded = true;
            }
        }
        var collectionModified = await SyncAlbum(manifestCollection, sizesToSync, cancellationToken);
        return collectionAdded || collectionModified;
    }

    private async Task<bool> SyncAlbum(Album manifestAlbum, string[] sizesToSync, CancellationToken cancellationToken = default)
    {
        var albumModified = false;
        var imageInfos = lightroomService.GetImageList(manifestAlbum.Id, cancellationToken);
        var albumImageIds = new List<string>();
        await foreach (var batch in imageInfos.Buffer(10))
        {
            cancellationToken.ThrowIfCancellationRequested();
            foreach (var imageInfo in batch)
            {
                cancellationToken.ThrowIfCancellationRequested();
                albumImageIds.Add(imageInfo.AssetId);
                bool syncImage = false;
                if (!manifestAlbum.Images.TryGetValue(imageInfo.AssetId, out var manifestImageInfo))
                {
                    logger.LogInformation("Asset {assetId} is not present in manifest, syncing", imageInfo.AssetId);

                    var metadata = metadataService.MapAdobeMetadataToManifestMetadata(imageInfo);

                    lock (manifestAlbum.Images)
                    {
                        manifestAlbum.Images.Add(imageInfo.AssetId, metadata);
                    }
                    manifestImageInfo = metadata;
                    albumModified = true;
                    syncImage = true;
                }

                if (manifestImageInfo.LastModified != imageInfo.LastModified)
                {
                    logger.LogInformation("Timestamp of asset {assetId} does not match manifest, syncing", imageInfo.AssetId);
                    manifestImageInfo.LastModified = imageInfo.LastModified;
                    syncImage = true;
                }

                if (syncImage)
                {
                    var imageSyncTasks = sizesToSync.Select(async size =>
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        using var imageStream = await lightroomService.GetImageStream(imageInfo.AssetId, size);
                        logger.LogInformation("Setting image {assetId} metadata {size}", imageInfo.AssetId, size);
                        using var updatedMetadataStream = await metadataService.SetImageMetadata(imageStream, imageInfo);

                        var imageName = $"{imageInfo.AssetId}.{size}.jpg";
                        await storageService.Store(imageName, updatedMetadataStream, JpgMimeType, cancellationToken);
                        manifestImageInfo.Hrefs.TryAdd(size, imageName);
                        albumModified = true;
                        logger.LogInformation("Synced {imageName}", imageName);
                    });
                    await Task.WhenAll(imageSyncTasks);
                }
            }
        }

        var toRemove = manifestAlbum.Images.Select(x => x.Key).Except(albumImageIds);
        foreach (var imageId in toRemove)
        {
            cancellationToken.ThrowIfCancellationRequested();
            logger.LogInformation("Image with id {imageId} exists in manifest but not in album, deleting", imageId);
            manifestAlbum.Images.Remove(imageId);
            albumModified = true;
            await storageService.DeleteBlobsStartingWith(imageId, cancellationToken);
        }

        return albumModified;
    }
}
