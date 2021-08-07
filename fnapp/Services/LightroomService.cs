using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.fnapp.Models;

namespace ldam.co.za.fnapp.Services
{
    public class LightroomService
    {
        private readonly ILightroomClient lightroomClient;
        private readonly Lazy<CatalogResponse> catalog;
        public LightroomService(ILightroomClient lightroomClient)
        {
            this.lightroomClient = lightroomClient;
            catalog = new Lazy<CatalogResponse>(() => lightroomClient.GetCatalog().Result);
        }

        public async IAsyncEnumerable<KeyValuePair<string, string>> GetAlbums()
        {
            string after = null;
            do
            {
                var albumsResponse = await lightroomClient.GetAlbums(this.catalog.Value.Id, after);
                Link next = null;
                albumsResponse.Links?.TryGetValue("next", out next);
                var afterHref = next?.Href;
                
                after = !string.IsNullOrWhiteSpace(afterHref) ? afterHref.Substring(afterHref.IndexOf('=')) : null;

                foreach(var albumResponse in albumsResponse.Resources.Where(x => x.Subtype == "collection"))
                {
                    yield return new KeyValuePair<string, string>(albumResponse.Id, albumResponse.Payload.Name);
                }
            }
            while(!string.IsNullOrEmpty(after));
        }
        public async IAsyncEnumerable<ImageInfo> GetImageList(string albumId)
        {
            string after = null;
            do
            {
                var albumAssetResponse = await lightroomClient.GetAlbumAssets(this.catalog.Value.Id, albumId, after);
                Link next = null;
                albumAssetResponse.Links?.TryGetValue("next", out next);
                var afterHref = next?.Href;
                after = !string.IsNullOrWhiteSpace(afterHref) ? afterHref.Substring(afterHref.IndexOf('=')) : null;
                foreach(var asset in albumAssetResponse.Resources)
                {
                    var exposureTimeArray = asset.Asset.Payload["xmp"]["exif"]["ExposureTime"];
                    var exposureNumerator = (decimal)exposureTimeArray[0];
                    var exposureDivisor = (decimal)exposureTimeArray[1]; 
                    var exposureTime = exposureNumerator == 1 && exposureDivisor > 1 ? $"1/{exposureDivisor}s" : $"{exposureNumerator/exposureDivisor}s";
                    var apertureArray = asset.Asset.Payload["xmp"]["exif"]["FNumber"];
                    var aperture = (decimal)apertureArray[0]/(decimal)apertureArray[1];
                    var make = (string)asset.Asset.Payload["xmp"]["tiff"]["Make"];
                    var model = (string)asset.Asset.Payload["xmp"]["tiff"]["Model"];
                    yield return new ImageInfo
                    {
                        AssetId = asset.Asset.Id,
                        FileName = (string)asset.Asset.Payload["importSource"]["fileName"],
                        CaptureDate = DateTime.Parse((string)asset.Asset.Payload["captureDate"]),
                        FileSize = (long)asset.Asset.Payload["importSource"]["fileSize"],
                        ShutterSpeed = exposureTime.ToString(),
                        FNumber = $"f/{aperture}",
                        FocalLength = $"{(int)asset.Asset.Payload["xmp"]["exif"]["FocalLength"].First}mm",
                        ISO = (string)asset.Asset.Payload["xmp"]["exif"]["ISOSpeedRatings"],
                        Lens = (string)asset.Asset.Payload["xmp"]["aux"]["Lens"],
                        Make = make,
                        Model = model,
                        Title = (string)asset.Asset.Payload["xmp"]["dc"]["title"],
                        Caption = (string)asset.Asset.Payload["xmp"]["dc"]["description"],
                    };
                }
            }
            while(!string.IsNullOrEmpty(after));
        }
    }
}