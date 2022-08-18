using ldam.co.za.lib;
using ldam.co.za.lib.Lightroom;

namespace ldam.co.za.fnapp.Services;

public interface ILightroomService
{
    IAsyncEnumerable<AlbumInfo> GetAlbums();
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
        catalog = new Lazy<CatalogResponse>(() => lightroomClient.GetCatalog().GetAwaiter().GetResult());
    }

    public async IAsyncEnumerable<AlbumInfo> GetAlbums()
    {
        string after = null;
        do
        {
            var albumsResponse = await lightroomClient.GetAlbums(this.catalog.Value.Id, after);
            Link next = null;
            albumsResponse.Links?.TryGetValue("next", out next);
            var afterHref = next?.Href;

            after = !string.IsNullOrWhiteSpace(afterHref) ? afterHref[afterHref.IndexOf('=')..] : null;

            foreach (var albumResponse in albumsResponse.Resources.Where(x => x.Subtype == "collection"))
            {
                yield return new AlbumInfo
                {
                    Id = albumResponse.Id,
                    Created = DateTime.Parse(albumResponse.Created),
                    Updated = DateTime.Parse(albumResponse.Updated),
                    Title = albumResponse.Payload.Name,
                    ParentId = albumResponse.Payload.Parent?.Id,
                };
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
            after = !string.IsNullOrWhiteSpace(afterHref) ? afterHref[afterHref.IndexOf('=')..] : null;
            foreach (var asset in albumAssetResponse.Resources)
            {
                var make = asset.Asset.Payload.Xmp.Tiff.Make;
                var model = asset.Asset.Payload.Xmp.Tiff.Model;
                yield return new ImageInfo
                {
                    AssetId = asset.Asset.Id,
                    FileName = asset.Asset.Payload.ImportSource.FileName,
                    CaptureDate = asset.Asset.Payload.CaptureDate,
                    FileSize = asset.Asset.Payload.ImportSource.FileSize,
                    ShutterSpeed = asset.Asset.Payload.Xmp.Exif.ExposureTime,
                    FNumber = asset.Asset.Payload.Xmp.Exif.FNumber,
                    FocalLength = asset.Asset.Payload.Xmp.Exif.FocalLength.First(),
                    ISO = asset.Asset.Payload.Xmp.Exif.ISOSpeedRatings,
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
