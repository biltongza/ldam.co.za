using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp
{
    public class AccessTokenProvider : IAccessTokenProvider
    {
        private readonly ISecretService secretService;
        private readonly IMemoryCache memoryCache;
        private readonly JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
        private readonly ILogger logger;

        public AccessTokenProvider(ISecretService secretService, IMemoryCache memoryCache, ILogger<AccessTokenProvider> logger)
        {
            this.secretService = secretService;
            this.memoryCache = memoryCache;
            this.logger = logger;
        }
        
        public async Task<string> GetAccessToken()
        {
            return await GetToken(lib.Constants.KeyVault.LightroomAccessToken);
        }

        public async Task<string> GetRefreshToken()
        {
            return await GetToken(lib.Constants.KeyVault.LightroomRefreshToken);
        }

        private async Task<string> GetToken(string key)
        {
            if(!memoryCache.TryGetValue<string>(key, out var accessToken))
            {
                logger.LogTrace("Cache miss for key {key}", key);
                accessToken = await secretService.GetSecret(key);
                var jwt = handler.ReadJwtToken(accessToken);
                var created_at = long.Parse(jwt.Claims.Single(x => x.Type == "created_at").Value); 
                var expires_in = long.Parse(jwt.Claims.Single(x => x.Type == "expires_in").Value);
                var createdTime = DateTimeOffset.FromUnixTimeMilliseconds(created_at);
                var expiresTime = createdTime.AddMilliseconds(expires_in);
                memoryCache.Set(key, accessToken, expiresTime.Subtract(TimeSpan.FromMinutes(5)));
            }
            return accessToken;
        }
    }
}