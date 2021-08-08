using System.IO;
using System.Threading.Tasks;

namespace ldam.co.za.fnapp.Services
{
    public interface IStorageService
    {
        Task Store(string name, Stream stream);
        Task<Stream> Get(string name);
    }
}