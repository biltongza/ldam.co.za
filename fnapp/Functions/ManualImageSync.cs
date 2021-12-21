using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions
{
    public class ManualImageSync
    {
        private readonly SyncService syncService;

        public ManualImageSync(SyncService syncService)
        {
            this.syncService = syncService;
        }

        [FunctionName(nameof(ManualImageSync))]
        public async Task Run([HttpTrigger(authLevel: AuthorizationLevel.Function, "post")] HttpRequest req, ILogger logger)
        {
            logger.LogInformation("Manual sync requested");
            bool.TryParse(req.Query["force"], out bool force);
            await syncService.Synchronize(force);
        }
    }
}