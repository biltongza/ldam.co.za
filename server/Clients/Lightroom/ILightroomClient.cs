using System.Threading.Tasks;

namespace ldam.co.za.server.Clients.Lightroom
{
    public interface ILightroomClient
    {
        Task<CatalogResponse> GetCatalog();
        Task<HealthResponse> GetHealth();
        Task<AlbumsResponse> GetAlbums(string catalogId, string after = null);
        Task<AlbumAssetResponse> GetAlbumAssets(string catalogId, string albumId, string after = null);
        Task<AssetResponse> GetAsset(string catalogId, string assetId);
    }
}