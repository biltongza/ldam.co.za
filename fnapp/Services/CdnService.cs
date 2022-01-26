using Azure.ResourceManager;
using Azure.ResourceManager.Cdn;
using Azure.ResourceManager.Cdn.Models;

public interface ICdnService
{
    Task ClearCache(string path);
}

public class CdnService : ICdnService
{
    private readonly CdnEndpoint cdnEndpoint;
    public CdnService(ArmClient armClient, string subscriptionId, string resourceGroup, string cdnProfileName, string endpointName)
    {
        this.cdnEndpoint = armClient.GetCdnEndpoint(CdnEndpoint.CreateResourceIdentifier(subscriptionId, resourceGroup, cdnProfileName, endpointName));
    }

    public async Task ClearCache(string path)
    {
        await cdnEndpoint.PurgeContentAsync(new PurgeOptions(new [] { path ?? "/*"}));
    }
}