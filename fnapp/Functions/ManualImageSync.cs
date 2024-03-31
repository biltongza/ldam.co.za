using ldam.co.za.fnapp.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions;

public class ManualImageSync
{
    private readonly SyncService syncService;
    private readonly ILogger logger;

    public ManualImageSync(SyncService syncService, ILogger<ManualImageSync> logger)
    {
        this.syncService = syncService;
        this.logger = logger;
    }

    [Function(nameof(ManualImageSync))]
    public async Task Run([HttpTrigger(authLevel: AuthorizationLevel.Function, "post")] HttpRequest req, CancellationToken cancellationToken = default)
    {
#pragma warning disable CA1806
        bool.TryParse(req.Query["force"], out bool force);
#pragma warning restore CA1806
        logger.LogInformation("Manual sync requested, force = {0}", force);
        await syncService.Synchronize(force, cancellationToken);
    }
}
