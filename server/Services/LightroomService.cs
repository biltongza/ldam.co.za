using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ldam.co.za.server.Clients.Lightroom;
using ldam.co.za.server.Models;

namespace ldam.co.za.server.Services
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
                    yield return new ImageInfo
                    {
                        FileName = (string)asset.Asset.Payload["importSource"]["fileName"],
                        CaptureDate = DateTime.Parse((string)asset.Asset.Payload["captureDate"]),
                        FileSize = (long)asset.Asset.Payload["importSource"]["fileSize"],
                    };
                }
            }
            while(!string.IsNullOrEmpty(after));
        }
    }
}