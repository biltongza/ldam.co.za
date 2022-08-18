using Azure.ResourceManager;
using Azure.ResourceManager.Cdn;
using Azure.ResourceManager.Cdn.Models;
using Microsoft.Extensions.Options;

public interface ICdnService
{
    Task ClearCache(string path, CancellationToken cancellationToken = default);
}

public class CdnService : ICdnService
{
    private readonly CdnEndpointResource cdnEndpoint;
    public CdnService(ArmClient armClient, IOptions<FunctionAppAzureResourceOptions> options)
    {
        var endpointResourceIdentifier = CdnEndpointResource.CreateResourceIdentifier(
            options.Value.CdnSubscriptionId, 
            options.Value.CdnResourceGroup, 
            options.Value.CdnProfileName, 
            options.Value.CdnEndpointName
        );
        this.cdnEndpoint = armClient.GetCdnEndpointResource(endpointResourceIdentifier);
    }

    public async Task ClearCache(string path, CancellationToken cancellationToken = default)
    {
        await cdnEndpoint.PurgeContentAsync(Azure.WaitUntil.Completed, new PurgeContent(new [] { path ?? "/*"}), cancellationToken: cancellationToken);
    }
}