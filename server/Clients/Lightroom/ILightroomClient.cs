using System.Threading.Tasks;

namespace ldam.co.za.server.Clients.Lightroom
{
    public interface ILightroomClient
    {
        Task<CatalogResponse> GetCatalog();
    }
}