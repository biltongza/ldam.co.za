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
        
        public async Task<string> GetAccessToken()
        {
            return await secretService.GetSecret(lib.Constants.KeyVault.LightroomAccessToken);
        }

        public async Task<string> GetRefreshToken()
        {
            return await secretService.GetSecret(lib.Constants.KeyVault.LightroomRefreshToken);
        }
    }
}