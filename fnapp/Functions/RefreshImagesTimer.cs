using ldam.co.za.fnapp.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions;

public class RefreshImagesTimer
{
    private readonly SyncService syncService;
    private readonly ILogger logger;

    public RefreshImagesTimer(SyncService syncService, ILogger<RefreshImagesTimer> logger)
    {
        this.syncService = syncService;
        this.logger = logger;
    }

    [Function(nameof(RefreshImagesTimer))]
    public async Task Run([TimerTrigger("0 */30 * * * *")] TimerInfo timerInfo, CancellationToken cancellationToken)
    {
        logger.LogInformation("RefreshImagesTimer fired");
        if (timerInfo.IsPastDue)
        {
            logger.LogInformation("Timer is late, skipping");
            return;
        }
        await syncService.Synchronize(force: false, cancellationToken: cancellationToken);
    }
}
