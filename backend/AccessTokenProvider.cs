using System.Linq;
using System.Threading.Tasks;
using ldam.co.za.lib.Lightroom;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace ldam.co.za.backend
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public AccessTokenProvider(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> GetAccessToken()
        {
            return await GetToken("access_token");
        }

        public async Task<string> GetRefreshToken()
        {
            return await GetToken("refresh_token");
        }

        private async Task<string> GetToken(string name)
        {
            var result = await httpContextAccessor.HttpContext.AuthenticateAsync();
            var accessTokens = result.Properties.GetTokens(); 
            var accessToken = accessTokens?.FirstOrDefault(t => t.Name == name)?.Value;
            return accessToken;
        }
    }
}