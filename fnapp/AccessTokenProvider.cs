using System.Threading.Tasks;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;

namespace ldam.co.za.fnapp
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly ISecretService secretService;

        public AccessTokenProvider(ISecretService secretService)
        {
            this.secretService = secretService;
        }
        
        public Task<string> GetAccessToken()
        {
            return Task.FromResult(secretService.GetSecret(lib.Constants.KeyVault.LightroomAccessToken));
        }
    }
}