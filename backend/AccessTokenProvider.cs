using System.Linq;
using System.Threading.Tasks;
using ldam.co.za.lib.Lightroom;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;

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
            var result = await this.httpContextAccessor.HttpContext.AuthenticateAsync();
            var accessToken = result.Properties.GetTokens()?.FirstOrDefault(t => t.Name == "access_token")?.Value;
            return accessToken;
        }
    }
}