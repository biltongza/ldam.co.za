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
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;

namespace ldam.co.za.fnapp.Services
{
    public class SyncService
    {
        private readonly ILightroomService lightroomService;
        private readonly IConfiguration configuration;
        private readonly IStorageService storageService;
        private readonly ILogger logger;
        private readonly JpegEncoder encoder;

        private const string ExifDateFormat = "yyyy:MM:dd HH:mm:ss";

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
            this.encoder = new JpegEncoder()
            {
                Quality = 100,
            };
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
            var manifestStream = await storageService.Get(manifestName);
            if (manifestStream == Stream.Null || force)
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
                if (!manifest.Albums.TryGetValue(album.Key, out var manifestAlbum))
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

                        var exposureTimeArray = imageInfo.ShutterSpeed;
                        var exposureNumerator = exposureTimeArray[0];
                        var exposureDivisor = exposureTimeArray[1];
                        var exposureTime = exposureNumerator == 1 && exposureDivisor > 1 ? $"1/{exposureDivisor}s" : $"{exposureNumerator / exposureDivisor}s";
                        var apertureArray = imageInfo.FNumber;
                        var aperture = decimal.Divide(apertureArray[0], apertureArray[1]);

                        var metadata = new ImageMetadata
                        {
                            CameraMake = imageInfo.Make,
                            CameraModel = imageInfo.Model,
                            Caption = imageInfo.Caption,
                            CaptureDate = imageInfo.CaptureDate,
                            FNumber = $"f/{aperture}",
                            FocalLength = $"{imageInfo.FocalLength}mm",
                            Id = imageInfo.AssetId,
                            ISO = imageInfo.ISO.ToString(),
                            LastModified = imageInfo.LastModified,
                            Lens = imageInfo.Lens,
                            ShutterSpeed = exposureTime,
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
                            logger.LogInformation("Setting image metadata {size}", size);
                            using var updatedMetadataStream = await SetImageMetadata(imageStream, imageInfo);

                            var imageName = $"{imageInfo.AssetId}.{size}.jpg";
                            await storageService.Store(imageName, updatedMetadataStream);
                            manifestImageInfo.Hrefs.TryAdd(size, imageName);
                            manifestModified = true;
                            logger.LogInformation("Synced {imageName}", imageName);
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

        private async Task<Stream> SetImageMetadata(Stream stream, ImageInfo imageInfo)
        {
            using var image = await Image.LoadAsync(stream);

            image.Metadata.ExifProfile = new ExifProfile();
            image.Metadata.ExifProfile.SetValue(ExifTag.Copyright, $"© Logan Dam {imageInfo.CaptureDate.Year}");
            image.Metadata.ExifProfile.SetValue(ExifTag.FNumber, new Rational((uint)imageInfo.FNumber[0], (uint)imageInfo.FNumber[1]));
            image.Metadata.ExifProfile.SetValue(ExifTag.ISOSpeedRatings, new[] { (ushort)imageInfo.ISO });
            image.Metadata.ExifProfile.SetValue(ExifTag.ExposureTime, new Rational((uint)imageInfo.ShutterSpeed[0], (uint)imageInfo.ShutterSpeed[1]));
            image.Metadata.ExifProfile.SetValue(ExifTag.FocalLength, new Rational((uint)imageInfo.FocalLength, 1));
            image.Metadata.ExifProfile.SetValue(ExifTag.Make, imageInfo.Make);
            image.Metadata.ExifProfile.SetValue(ExifTag.Model, imageInfo.Model);
            image.Metadata.ExifProfile.SetValue(ExifTag.LensModel, imageInfo.Lens);
            image.Metadata.ExifProfile.SetValue(ExifTag.DateTimeOriginal, imageInfo.CaptureDate.ToString(ExifDateFormat));
            image.Metadata.ExifProfile.SetValue(ExifTag.ApertureValue, new Rational((uint)imageInfo.FNumber[0], (uint)imageInfo.FNumber[1]));

            var updatedStream = new MemoryStream();
            await image.SaveAsJpegAsync(updatedStream, this.encoder);
            updatedStream.Seek(0, SeekOrigin.Begin);
            return updatedStream;
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