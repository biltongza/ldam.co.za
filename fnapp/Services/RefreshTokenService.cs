using System.IdentityModel.Tokens.Jwt;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ldam.co.za.fnapp.Services;

public class RefreshTokenService
{
    private readonly TimeSpan refreshWindow;
    private readonly ISecretService secretService;
    private readonly IClock clock;
    private readonly ILogger logger;
    private readonly ILightroomTokenService lightroomTokenService;
    private readonly JwtSecurityTokenHandler handler = new();

    public RefreshTokenService(
        ISecretService secretService,
        IClock clock,
        IOptionsSnapshot<FunctionAppLightroomOptions> options,
        ILogger<RefreshTokenService> logger,
        ILightroomTokenService lightroomTokenService)
    {
        this.secretService = secretService;
        this.clock = clock;
        this.logger = logger;
        this.lightroomTokenService = lightroomTokenService;
        refreshWindow = TimeSpan.FromMinutes(int.Parse(options.Value.RefreshTokenWindowMinutes));
    }

    public async Task RefreshAccessToken()
    {
        var accessToken = await secretService.GetSecret(lib.Constants.KeyVault.LightroomAccessToken);
        var refreshToken = await secretService.GetSecret(lib.Constants.KeyVault.LightroomRefreshToken);

        if (!IsTokenStillValid(refreshToken))
        {
            throw new InvalidOperationException("Refresh token has expired");
        }

        if (IsTokenStillValid(accessToken))
        {
            logger.LogInformation("Access token is still valid");
            return;
        }

        logger.LogInformation("Access token has expired, refreshing");

        await lightroomTokenService.UpdateAccessToken();
    }

    private bool IsTokenStillValid(string? token)
    {
        if (token is null)
        {
            return false;
        }
        var jwt = handler.ReadJwtToken(token);
        var now = clock.Now();
        var created_at = long.Parse(jwt.Claims.Single(x => x.Type == "created_at").Value);
        var expires_in = long.Parse(jwt.Claims.Single(x => x.Type == "expires_in").Value);
        var createdTime = DateTimeOffset.FromUnixTimeMilliseconds(created_at);
        var expiresTime = createdTime.AddMilliseconds(expires_in);
        var difference = expiresTime.Subtract(now);
        return difference.CompareTo(refreshWindow) > 0;
    }

}
