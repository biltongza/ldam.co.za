using ldam.co.za.fnapp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions;

public class RefreshAccessTokenFunc
{
    private readonly RefreshTokenService refreshTokenService;
    public RefreshAccessTokenFunc(RefreshTokenService refreshTokenService)
    {
        this.refreshTokenService = refreshTokenService;
    }

    [FunctionName(nameof(RefreshAccessTokenFunc))]
    #pragma warning disable IDE0060
    public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo, ILogger logger)
    #pragma warning restore IDE0060
    {
        await refreshTokenService.RefreshAccessToken();
    }
}
