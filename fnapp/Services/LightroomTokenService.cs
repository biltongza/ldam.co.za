using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Configuration;
using AspNet.Security.OAuth.AdobeIO;

namespace ldam.co.za.fnapp.Services
{
    public interface ILightroomTokenService
    {
        Task UpdateAccessToken();
    }

    public class LightroomTokenService : ILightroomTokenService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ISecretService secretService;
        private readonly IConfiguration configuration;

        public LightroomTokenService(IHttpClientFactory httpClientFactory, ISecretService secretService, IConfiguration configuration)
        {
            this.httpClientFactory = httpClientFactory;
            this.secretService = secretService;
            this.configuration = configuration;
        }

        public async Task UpdateAccessToken()
        {
            var refreshToken = await secretService.GetSecret(lib.Constants.KeyVault.LightroomRefreshToken);
            var clientSecret = await secretService.GetSecret(lib.Constants.KeyVault.LightroomClientSecret);
            var clientId = configuration[Constants.Configuration.Adobe.AuthClientId];

            var pairs = new Dictionary<string, string>()
            {
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "grant_type", "refresh_token" },
                { "refresh_token", refreshToken }
            };
            var content = new FormUrlEncodedContent(pairs);

            using var httpClient = httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(AdobeIOAuthenticationDefaults.TokenEndpoint, content);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            var payload = await System.Text.Json.JsonDocument.ParseAsync(responseStream);
            var newAccessToken = payload.RootElement.GetProperty("access_token").GetString();
            if (!string.IsNullOrEmpty(newAccessToken))
            {
                await secretService.SetSecret(lib.Constants.KeyVault.LightroomAccessToken, newAccessToken);
            }

            var newRefreshToken = payload.RootElement.GetProperty("refresh_token").GetString();
            if (!string.IsNullOrEmpty(newRefreshToken))
            {
                await secretService.SetSecret(lib.Constants.KeyVault.LightroomRefreshToken, newAccessToken);
            }
        }

    }
}