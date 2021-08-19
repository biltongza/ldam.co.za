using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions
{
    public class RefreshImagesFunc
    {
        private readonly SyncService syncService;

        public RefreshImagesFunc(SyncService syncService)
        {
            this.syncService = syncService;
        }

        [Function(nameof(RefreshImagesFunc))]
        public async Task Run([TimerTrigger("0 */5 * * * *")] TimerInfo timerInfo, FunctionContext functionContext)
        {
            var logger = functionContext.GetLogger<RefreshImagesFunc>();
            if(timerInfo.IsPastDue)
            {
                logger.LogInformation("Timer is late, skipping");
                return;
            }
            await syncService.SyncImages();
        }
    }
}