using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions
{
    public class RefreshAccessTokenFunc
    {
        RefreshTokenService refreshTokenService;
        public RefreshAccessTokenFunc(RefreshTokenService refreshTokenService)
        {
            this.refreshTokenService = refreshTokenService;
        }

        [FunctionName(nameof(RefreshAccessTokenFunc))]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo, ILogger logger)
        {
            await refreshTokenService.RefreshAccessToken();
        }
    }
}