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

        public SyncService(
            ILightroomService lightroomService,
            IConfiguration configuration,
            IStorageService storageService,
            ILogger<SyncService> logger)
        {
            this.lightroomService = lightroomService;
            this.configuration = configuration;
            this.storageService = storageService;
            this.logger = logger;
        }

        public async Task SyncImages()
        {
            var albumIdsToSync = configuration[Constants.Configuration.Adobe.AlbumIds].Split(',');
            var sizesToSync = configuration[Constants.Configuration.Adobe.SizesToSync].Split(',');

            Manifest manifest = null;
            bool manifestModified = false;
            var manifestName = "manifest.json";
            var manifestStream = await storageService.Get(manifestName);
            if (manifestStream == Stream.Null)
            {
                manifest = new Manifest();
                manifestModified = true;
            }
            else
            {
                manifest = await JsonSerializer.DeserializeAsync<Manifest>(manifestStream);
            }
            
            var albumsToSync = await lightroomService.GetAlbums()
                .Where(album => albumIdsToSync.Contains(album.Key))
                .ToListAsync();

            var tasks = albumsToSync.Select(async album =>
            {
                if(!manifest.Albums.TryGetValue(album.Key, out var manifestAlbum))
                {
                    logger.LogInformation("Album {albumId} not present in manifest, syncing", album);
                    manifestAlbum = new Album
                    {
                        Id = album.Key,
                        Title = album.Value,
                    };
                    manifest.Albums.Add(album.Key, manifestAlbum);
                    manifestModified = true;
                }

                var imageInfos = lightroomService.GetImageList(album.Key);
                var albumImageIds = new List<string>();
                await foreach (var imageInfo in imageInfos)
                {
                    albumImageIds.Add(imageInfo.AssetId);
                    bool syncImage = false;
                    if (!manifestAlbum.Images.TryGetValue(imageInfo.AssetId, out var manifestImageInfo))
                    {
                        logger.LogInformation("Asset {assetId} is not present in manifest, syncing", imageInfo.AssetId);
                        
                        var gcd = GreatestCommonDenominator(imageInfo.Width, imageInfo.Height);
                        
                        var metadata = new ImageMetadata
                        {
                            CameraMake = imageInfo.Make,
                            CameraModel = imageInfo.Model,
                            Caption = imageInfo.Caption,
                            CaptureDate = imageInfo.CaptureDate,
                            FNumber = imageInfo.FNumber,
                            FocalLength = imageInfo.FocalLength,
                            Id = imageInfo.AssetId,
                            ISO = imageInfo.ISO,
                            LastModified = imageInfo.LastModified,
                            Lens = imageInfo.Lens,
                            ShutterSpeed = imageInfo.ShutterSpeed,
                            Title = imageInfo.Title,
                            Width = imageInfo.Width,
                            Height = imageInfo.Height,
                            AspectRatio = $"{imageInfo.Width / gcd}:{imageInfo.Height / gcd}",
                        };
                        
                        manifestAlbum.Images.Add(imageInfo.AssetId, metadata);
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
                        foreach (var size in sizesToSync)
                        {
                            using var imageStream = await lightroomService.GetImageStream(imageInfo.AssetId, size);
                            var imageName = $"{imageInfo.AssetId}.{size}.jpg";
                            await storageService.Store(imageName, imageStream);
                            manifestImageInfo.Hrefs.TryAdd(size, imageName);
                            manifestModified = true;
                            logger.LogInformation("Synced {imageName}", imageName);
                        }
                    }
                }
                var toRemove = manifestAlbum.Images.Select(x => x.Key).Except(albumImageIds);
                foreach(var imageId in toRemove)
                {
                    logger.LogInformation("Image with id {imageId} exists in manifest but not in album, deleting", imageId);
                    manifestAlbum.Images.Remove(imageId);
                    manifestModified = true;
                    await this.storageService.DeleteBlobsStartingWith(imageId);
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

        static int GreatestCommonDenominator(int a, int b)
        {
            if (b == 0)
            {
                return a;
            }
            return GreatestCommonDenominator(b, a % b);
        }
    }
}