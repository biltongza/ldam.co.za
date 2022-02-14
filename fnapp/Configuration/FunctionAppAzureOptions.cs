public class FunctionAppAzureResourceOptions : ldam.co.za.lib.Configuration.AzureResourceOptions
{
    public string CdnSubscriptionId { get; init; }
    public string CdnResourceGroup { get; init; }
    public string CdnProfileName { get; init; }
    public string CdnEndpointName { get; init; }
}