using System.Threading.Tasks;

namespace ldam.co.za.lib.Services
{
    public interface ISecretService
    {
        Task<string> GetSecret(string key);
        Task SetSecret(string key, string value);
    }
}