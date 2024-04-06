public class FunctionAppAzureResourceOptions : ldam.co.za.lib.Configuration.AzureResourceOptions
{
    public required string CdnSubscriptionId { get; init; }
    public required string CdnResourceGroup { get; init; }
    public required string CdnProfileName { get; init; }
    public required string CdnEndpointName { get; init; }
}