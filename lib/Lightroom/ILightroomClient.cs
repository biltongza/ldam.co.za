namespace ldam.co.za.lib.Lightroom;

public interface ILightroomClient
{
    Task<string> GetCatalogId(CancellationToken cancellationToken = default);
    Task<AlbumsResponse> GetAlbums(string catalogId, string? after = null, CancellationToken cancellationToken = default);
    Task<AlbumAssetResponse> GetAlbumAssets(string catalogId, string albumId, string? after = null, CancellationToken cancellationToken = default);
    Task<AssetResponse> GetAsset(string catalogId, string assetId, CancellationToken cancellationToken = default);
    Task<Stream> GetImageStream(string catalogId, string assetId, string size, CancellationToken cancellationToken = default);
}
