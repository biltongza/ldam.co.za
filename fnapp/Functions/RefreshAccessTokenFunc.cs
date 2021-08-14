using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;

namespace ldam.co.za.fnapp.Functions
{
    public class RefreshAccessTokenFunc
    {
        RefreshTokenService refreshTokenService;
        public RefreshAccessTokenFunc(RefreshTokenService refreshTokenService)
        {
            this.refreshTokenService = refreshTokenService;
        }

        [Function(nameof(RefreshAccessTokenFunc))]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo, FunctionContext functionContext)
        {
            await refreshTokenService.RefreshAccessToken();
        }
    }
}