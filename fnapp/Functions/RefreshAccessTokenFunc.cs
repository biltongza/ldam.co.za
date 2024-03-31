using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions;

public class RefreshAccessTokenFunc
{
    private readonly RefreshTokenService refreshTokenService;
    private readonly ILogger logger;

    public RefreshAccessTokenFunc(RefreshTokenService refreshTokenService, ILogger<RefreshAccessTokenFunc> logger)
    {
        this.refreshTokenService = refreshTokenService;
        this.logger = logger;
    }

    [Function(nameof(RefreshAccessTokenFunc))]
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo)
    {
        logger.LogInformation("RefreshAccessToken fired");
        await refreshTokenService.RefreshAccessToken();
    }
}
