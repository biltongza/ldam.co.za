using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za.server
{
    public class AccessTokenProvider
    {
        private const string AccessTokenKey = "ACCESS_TOKEN";
        private readonly IMemoryCache memoryCache;
        private readonly IHttpContextAccessor httpContextAccessor;

        public string AccessToken
        {
            get => GetAccessToken().Result;
        }

        public AccessTokenProvider(IMemoryCache memoryCache, IHttpContextAccessor httpContextAccessor)
        {
            this.memoryCache = memoryCache;
            this.httpContextAccessor = httpContextAccessor;
        }

        private async Task<string> GetAccessToken()
        {
            if (!this.memoryCache.TryGetValue<string>(AccessTokenKey, out string accessToken))
            {
                var result = await this.httpContextAccessor.HttpContext.AuthenticateAsync();
                accessToken = result.Properties.GetTokens()?.FirstOrDefault(t => t.Name == "access_token")?.Value;
                this.memoryCache.Set(AccessTokenKey, accessToken);
                
            }
            return accessToken;
        }
    }
}