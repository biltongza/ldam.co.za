using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.IO;
using ldam.co.za.fnapp.Models;
using System;
using System.Text.Json;
using Microsoft.Azure.Functions.Worker;

namespace ldam.co.za.fnapp.Functions
{
    public class RefreshImagesFunc
    {
        private readonly LightroomService lightroomService;
        private readonly IConfiguration configuration;
        private readonly StorageService storageService;

        public RefreshImagesFunc(
            LightroomService lightroomService, 
            IConfiguration configuration,
            StorageService storageService)
        {
            this.lightroomService = lightroomService;
            this.configuration = configuration;
            this.storageService = storageService;
        }

        [Function(nameof(RefreshImagesFunc))]
        public async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo timerInfo, FunctionContext functionContext)
        {
            var log = functionContext.GetLogger<RefreshImagesFunc>();
            var albumsToSync = configuration[Constants.Configuration.Adobe.AlbumIds].Split(',');
            var sizesToSync = configuration[Constants.Configuration.Adobe.SizesToSync].Split(',');
            var tasks = albumsToSync.Select(async albumId =>
            {
                Manifest manifest = null;
                bool manifestModified = false;
                var manifestName = $"{albumId}.manifest.json";
                var manifestStream = await storageService.Get(manifestName);
                if(manifestStream == Stream.Null)
                {
                    manifest = new Manifest();
                    manifestModified = true;
                }
                else
                {
                    manifest = await JsonSerializer.DeserializeAsync<Manifest>(manifestStream);
                }

                var imageInfos = lightroomService.GetImageList(albumId);
                await foreach(var imageInfo in imageInfos)
                {
                    bool syncImage = false;
                    if(!manifest.Images.TryGetValue(imageInfo.AssetId, out var manifestImageInfo))
                    {
                        log.LogInformation("Asset {assetId} is not present in manifest, syncing");
                        manifest.Images.Add(imageInfo.AssetId, imageInfo);
                        manifestImageInfo = imageInfo;
                        manifestModified = true;
                        syncImage = true;
                    }
                    
                    if(manifestImageInfo.LastModified != imageInfo.LastModified)
                    {
                        log.LogInformation("Timestamp of asset {assetId} does not match manifest, syncing", imageInfo.AssetId);
                        syncImage = true;
                    }

                    if(syncImage)
                    {
                        foreach (var size in sizesToSync)
                        {
                            using var imageStream = await lightroomService.GetImageStream(imageInfo.AssetId, size);
                            var imageName = $"{imageInfo.AssetId}.{size}.jpg";
                            await storageService.Store(imageName, imageStream);
                            log.LogInformation("Synced {imageName}", imageName);
                        }
                    }
                }

                if(manifestModified)
                {
                    manifest.LastModified = DateTime.Now;
                    var serializedManifest = JsonSerializer.SerializeToUtf8Bytes(manifest);
                    using var serializedStream = new MemoryStream(serializedManifest);
                    await storageService.Store(manifestName, serializedStream);
                    log.LogInformation("Manifest {manifestName} updated", manifestName);
                }
            }).ToArray();

            await Task.WhenAll(tasks);
        }
    }
}