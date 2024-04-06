namespace ldam.co.za.lib.Configuration;
public class AzureResourceOptions
{
    public required Uri KeyVaultUri { get; init; }
    public required Uri BlobStorageUri { get; init; }
    public required string BlobContainer { get; init; }
}