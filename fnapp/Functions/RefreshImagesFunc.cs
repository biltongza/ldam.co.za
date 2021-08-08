using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;

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
            await syncService.SyncImages();
        }
    }
}