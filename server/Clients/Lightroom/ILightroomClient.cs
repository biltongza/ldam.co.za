using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ldam.co.za.server.Clients.Lightroom
{
    public interface ILightroomClient
    {
        Task<CatalogResponse> GetCatalog();
        Task<object> GetHealth();
    }
}