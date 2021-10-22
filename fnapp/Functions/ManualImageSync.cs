using System.Threading.Tasks;
using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
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

        [Function(nameof(ManualImageSync))]
        public async Task Run([HttpTrigger(authLevel: AuthorizationLevel.Function, "post")] HttpRequestData req, FunctionContext context)
        {
            var logger = context.GetLogger<ManualImageSync>();
            logger.LogInformation("Manual sync requested");
            var query = System.Web.HttpUtility.ParseQueryString(req.Url.Query);
            bool.TryParse(query["force"], out bool force);
            await syncService.Synchronize(force);
        }
    }
}