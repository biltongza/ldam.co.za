using AspNet.Security.OAuth.AdobeIO;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ldam.co.za.fnapp.Services;

public interface ILightroomTokenService
{
    Task UpdateAccessToken();
}

public class LightroomTokenService : ILightroomTokenService
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ISecretService secretService;
    private readonly IOptions<FunctionAppLightroomOptions> options;
    private readonly ILogger logger;

    public LightroomTokenService(
        IHttpClientFactory httpClientFactory,
        ISecretService secretService,
        IOptions<FunctionAppLightroomOptions> options,
        ILogger<LightroomTokenService> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.secretService = secretService;
        this.options = options;
        this.logger = logger;
    }

    public async Task UpdateAccessToken()
    {
        var refreshToken = await secretService.GetSecret(lib.Constants.KeyVault.LightroomRefreshToken) ?? throw new InvalidOperationException("Refresh token is null!");
        var clientSecret = await secretService.GetSecret(lib.Constants.KeyVault.LightroomClientSecret) ?? throw new InvalidOperationException("Client secret is null!");

        var pairs = new Dictionary<string, string>()
            {
                { "client_id", options.Value.ClientId },
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
            logger.LogInformation("Updating access token");
            await secretService.SetSecret(lib.Constants.KeyVault.LightroomAccessToken, newAccessToken);
        }

        var newRefreshToken = payload.RootElement.GetProperty("refresh_token").GetString();
        if (!string.IsNullOrEmpty(newRefreshToken))
        {
            logger.LogInformation("Updating refresh token");
            await secretService.SetSecret(lib.Constants.KeyVault.LightroomRefreshToken, newRefreshToken);
        }
    }

}
