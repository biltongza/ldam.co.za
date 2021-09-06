using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Models;
using System.IO;

namespace ldam.co.za.fnapp.Services
{
    public interface ILightroomService
    {
        IAsyncEnumerable<KeyValuePair<string, string>> GetAlbums();
        IAsyncEnumerable<ImageInfo> GetImageList(string albumId);
        Task<Stream> GetImageStream(string assetId, string size);
    }

    public class LightroomService : ILightroomService
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

                foreach (var albumResponse in albumsResponse.Resources.Where(x => x.Subtype == "collection"))
                {
                    yield return new KeyValuePair<string, string>(albumResponse.Id, albumResponse.Payload.Name);
                }
            }
            while (!string.IsNullOrEmpty(after));
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
                foreach (var asset in albumAssetResponse.Resources)
                {
                    var exposureTimeArray = asset.Asset.Payload.Xmp.Exif.ExposureTime;
                    var exposureNumerator = exposureTimeArray[0];
                    var exposureDivisor = exposureTimeArray[1];
                    var exposureTime = exposureNumerator == 1 && exposureDivisor > 1 ? $"1/{exposureDivisor}s" : $"{exposureNumerator / exposureDivisor}s";
                    var apertureArray = asset.Asset.Payload.Xmp.Exif.FNumber;
                    var aperture = apertureArray[0] / apertureArray[1];
                    var make = asset.Asset.Payload.Xmp.Tiff.Make;
                    var model = asset.Asset.Payload.Xmp.Tiff.Model;
                    yield return new ImageInfo
                    {
                        AssetId = asset.Asset.Id,
                        FileName = asset.Asset.Payload.ImportSource.FileName,
                        CaptureDate = asset.Asset.Payload.CaptureDate,
                        FileSize = asset.Asset.Payload.ImportSource.FileSize,
                        ShutterSpeed = exposureTime.ToString(),
                        FNumber = $"f/{aperture}",
                        FocalLength = $"{asset.Asset.Payload.Xmp.Exif.FocalLength.First()}mm",
                        ISO = asset.Asset.Payload.Xmp.Exif.ISOSpeedRatings.ToString(),
                        Lens = asset.Asset.Payload.Xmp.Aux.Lens,
                        Make = make,
                        Model = model,
                        Title = asset.Asset.Payload.Xmp.Dc.Title,
                        Caption = asset.Asset.Payload.Xmp.Dc.Description,
                        LastModified = DateTime.Parse(asset.Asset.Updated),
                        Width = asset.Asset.Payload.Develop.CroppedWidth,
                        Height = asset.Asset.Payload.Develop.CroppedHeight,
                    };
                }
            }
            while (!string.IsNullOrEmpty(after));
        }

        public async Task<Stream> GetImageStream(string assetId, string size)
        {
            var stream = await lightroomClient.GetImageStream(this.catalog.Value.Id, assetId, size);

            return stream;
        }
    }
}