namespace ldam.co.za.lib.Configuration;
public class AzureResourceOptions
{
    public Uri KeyVaultUri { get; init; }
    public Uri BlobStorageUri { get; init; }
    public string BlobContainer { get; init; }
}