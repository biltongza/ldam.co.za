using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using ldam.co.za.contracts;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Services
{
    public class SyncService
    {
        private readonly ILightroomService lightroomService;
        private readonly IConfiguration configuration;
        private readonly IStorageService storageService;
        private readonly ILogger logger;
        private readonly IMetadataService metadataService;

        public SyncService(
            ILightroomService lightroomService,
            IConfiguration configuration,
            IStorageService storageService,
            ILogger<SyncService> logger,
            IMetadataService metadataService)
        {
            this.lightroomService = lightroomService;
            this.configuration = configuration;
            this.storageService = storageService;
            this.logger = logger;
            this.metadataService = metadataService;
        }

        public async Task Synchronize(bool force)
        {
            if (force)
            {
                logger.LogInformation("Forcing full refresh");
            }
            var albumIdsToSync = configuration[Constants.Configuration.Adobe.AlbumIds].Split(',');
            var sizesToSync = configuration[Constants.Configuration.Adobe.SizesToSync].Split(',');

            Manifest manifest = null;
            bool manifestModified = false;
            var manifestName = "manifest.json";
            using (var manifestStream = await storageService.Get(manifestName))
            {
                if (manifestStream == Stream.Null || force)
                {
                    manifest = new Manifest();
                    manifestModified = true;
                }
                else
                {
                    manifest = await JsonSerializer.DeserializeAsync<Manifest>(manifestStream);
                }
            }
            var albumsToSync = await lightroomService.GetAlbums()
                .Where(album => albumIdsToSync.Contains(album.Key))
                .ToListAsync();

            var tasks = albumsToSync.Select(async album =>
            {
                if (!manifest.Albums.TryGetValue(album.Key, out var manifestAlbum))
                {
                    logger.LogInformation("Album {albumId} not present in manifest, syncing", album);
                    manifestAlbum = new Album
                    {
                        Id = album.Key,
                        Title = album.Value,
                    };
                    lock (manifest.Albums)
                    {
                        manifest.Albums.Add(album.Key, manifestAlbum);
                    }
                    manifestModified = true;
                }

                var imageInfos = lightroomService.GetImageList(album.Key);
                var albumImageIds = new List<string>();
                await foreach (var batch in imageInfos.Buffer(10))
                {
                    foreach (var imageInfo in batch)
                    {
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
                            manifestModified = true;
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
                                using var imageStream = await lightroomService.GetImageStream(imageInfo.AssetId, size);
                                logger.LogInformation("Setting image {assetId} metadata {size}", imageInfo.AssetId, size);
                                using var updatedMetadataStream = await metadataService.SetImageMetadata(imageStream, imageInfo);

                                var imageName = $"{imageInfo.AssetId}.{size}.jpg";
                                await storageService.Store(imageName, updatedMetadataStream);
                                manifestImageInfo.Hrefs.TryAdd(size, imageName);
                                manifestModified = true;
                                logger.LogInformation("Synced {imageName}", imageName);
                            });
                            await Task.WhenAll(imageSyncTasks);
                        }
                    }
                }
                var toRemove = manifestAlbum.Images.Select(x => x.Key).Except(albumImageIds);
                foreach (var imageId in toRemove)
                {
                    logger.LogInformation("Image with id {imageId} exists in manifest but not in album, deleting", imageId);
                    manifestAlbum.Images.Remove(imageId);
                    manifestModified = true;
                    await storageService.DeleteBlobsStartingWith(imageId);
                }
            }).ToArray();

            await Task.WhenAll(tasks);

            if (manifestModified)
            {
                manifest.LastModified = DateTime.Now;
                var serializedManifest = JsonSerializer.SerializeToUtf8Bytes(manifest);
                using var serializedStream = new MemoryStream(serializedManifest);
                await storageService.Store(manifestName, serializedStream);
                logger.LogInformation("Manifest {manifestName} updated", manifestName);
            }
        }
    }
}