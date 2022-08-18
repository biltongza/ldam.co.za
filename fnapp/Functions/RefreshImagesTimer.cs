using ldam.co.za.fnapp.Services;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Functions;

public class RefreshImagesTimer
{
    private readonly SyncService syncService;

    public RefreshImagesTimer(SyncService syncService)
    {
        this.syncService = syncService;
    }

    [FunctionName(nameof(RefreshImagesTimer))]
    public async Task Run([TimerTrigger("0 */30 * * * *")] TimerInfo timerInfo, ILogger logger, CancellationToken cancellationToken)
    {
        if (timerInfo.IsPastDue)
        {
            logger.LogInformation("Timer is late, skipping");
            return;
        }
        await syncService.Synchronize(force: false, cancellationToken: cancellationToken);
    }
}
