using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

namespace ldam.co.za.lib.Services;

public interface ISecretService
{
    Task<string> GetSecret(string key);
    Task SetSecret(string key, string value);
}
public class SecretService : ISecretService
{
    private readonly SecretClient secretClient;
    public SecretService(string keyVaultUri)
    {
        var credential = new ChainedTokenCredential(
#if !DEBUG
                new ManagedIdentityCredential(),
#endif
                new AzureCliCredential()
        );
        this.secretClient = new SecretClient(new Uri(keyVaultUri), credential);
    }

    public async Task<string> GetSecret(string key)
    {
        var secretResponse = await secretClient.GetSecretAsync(key);
        return secretResponse?.Value?.Value;
    }

    public async Task SetSecret(string key, string value)
    {
        await secretClient.SetSecretAsync(key, value);
    }
}
