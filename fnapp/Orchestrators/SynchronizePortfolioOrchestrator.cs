using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.DurableTask;
using Microsoft.DurableTask.Client;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Orchestrators;

public class SynchronizePortfolioOrchestrator
{
    [Function(nameof(TriggerSynchronizePortfolio))]
    public static async Task<HttpResponseData> TriggerSynchronizePortfolio([HttpTrigger] HttpRequestData req, [DurableClient] DurableTaskClient durableTaskClient)
    {
        var instanceId = await durableTaskClient.ScheduleNewOrchestrationInstanceAsync(nameof(SynchronizePortfolio));

        return await durableTaskClient.CreateCheckStatusResponseAsync(req, instanceId);
    }

    [Function(nameof(SynchronizePortfolio))]
    public static async Task SynchronizePortfolio([OrchestrationTrigger] TaskOrchestrationContext context)
    {
        var logger = context.CreateReplaySafeLogger<SynchronizePortfolioOrchestrator>();
        logger.LogInformation(nameof(SynchronizePortfolioOrchestrator));
    }
}