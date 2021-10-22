using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions
{
    public class RefreshImagesTimer
    {
        private readonly SyncService syncService;

        public RefreshImagesTimer(SyncService syncService)
        {
            this.syncService = syncService;
        }

        [Function(nameof(RefreshImagesTimer))]
        public async Task Run([TimerTrigger("0 */30 * * * *")] TimerInfo timerInfo, FunctionContext functionContext)
        {
            var logger = functionContext.GetLogger<RefreshImagesTimer>();
            if(timerInfo.IsPastDue)
            {
                logger.LogInformation("Timer is late, skipping");
                return;
            }
            await syncService.Synchronize(force: false);
        }
    }
}