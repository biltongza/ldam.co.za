using System.Runtime.CompilerServices;
using ldam.co.za.lib.Lightroom;
using ImageInfo = ldam.co.za.lib.ImageInfo;

namespace ldam.co.za.fnapp.Services;

public interface ILightroomService
{
    IAsyncEnumerable<AlbumInfo> GetAlbums(CancellationToken cancellationToken = default);
    IAsyncEnumerable<ImageInfo> GetImageList(string albumId, CancellationToken cancellationToken = default);
    Task<Stream> GetImageStream(string assetId, string size, CancellationToken cancellationToken = default);
}

public class LightroomService : ILightroomService
{
    private readonly ILightroomClient lightroomClient;
    private readonly Lazy<string> catalogId;
    public LightroomService(ILightroomClient lightroomClient)
    {
        this.lightroomClient = lightroomClient;
        catalogId = new Lazy<string>(() => lightroomClient.GetCatalogId().GetAwaiter().GetResult());
    }

    public async IAsyncEnumerable<AlbumInfo> GetAlbums([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? after = null;
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var albumsResponse = await lightroomClient.GetAlbums(this.catalogId.Value, after, cancellationToken);
            Link? next = null;
            albumsResponse.Links?.TryGetValue("next", out next);
            var afterHref = next?.Href;

            after = string.IsNullOrWhiteSpace(afterHref) ? null : afterHref[afterHref.IndexOf('=')..];

            foreach (var albumResponse in albumsResponse.Resources.Where(x => x.Subtype == "collection"))
            {
                cancellationToken.ThrowIfCancellationRequested();

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
    public async IAsyncEnumerable<ImageInfo> GetImageList(string albumId, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        string? after = null;
        do
        {
            cancellationToken.ThrowIfCancellationRequested();
            var albumAssetResponse = await lightroomClient.GetAlbumAssets(this.catalogId.Value, albumId, after, cancellationToken);
            Link? next = null;
            albumAssetResponse.Links?.TryGetValue("next", out next);
            var afterHref = next?.Href;
            after = string.IsNullOrWhiteSpace(afterHref) ? null : afterHref[afterHref.IndexOf('=')..];
            foreach (var asset in albumAssetResponse.Resources)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var model = asset.Asset.Payload.Xmp?.Tiff?.Model;
                yield return new ImageInfo
                {
                    AssetId = asset.Asset.Id,
                    CaptureDate = asset.Asset.Payload.CaptureDate,
                    ShutterSpeed = asset.Asset.Payload.Xmp?.Exif?.ExposureTime,
                    FNumber = asset.Asset.Payload.Xmp?.Exif?.FNumber,
                    FocalLength = asset.Asset.Payload.Xmp?.Exif?.FocalLength?.FirstOrDefault(),
                    ISO = asset.Asset.Payload.Xmp?.Exif?.ISOSpeedRatings,
                    Lens = asset.Asset.Payload.Xmp?.Aux?.Lens,
                    Model = model,
                    Title = asset.Asset.Payload.Xmp?.Dc?.Title,
                    Caption = asset.Asset.Payload.Xmp?.Dc?.Description,
                    LastModified = DateTime.Parse(asset.Asset.Updated),
                    Width = asset.Asset.Payload.Develop.CroppedWidth,
                    Height = asset.Asset.Payload.Develop.CroppedHeight,
                };
            }
        }
        while (!string.IsNullOrEmpty(after));
    }

    public async Task<Stream> GetImageStream(string assetId, string size, CancellationToken cancellationToken = default)
    {
        var stream = await lightroomClient.GetImageStream(this.catalogId.Value, assetId, size, cancellationToken);

        return stream;
    }
}
