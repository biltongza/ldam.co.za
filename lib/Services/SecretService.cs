using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using ldam.co.za.lib.Configuration;
using Microsoft.Extensions.Options;

namespace ldam.co.za.lib.Services;

public interface ISecretService
{
    Task<string?> GetSecret(string key);
    Task SetSecret(string key, string value);
}
public class SecretService : ISecretService
{
    private readonly SecretClient secretClient;
    public SecretService(IOptions<AzureResourceOptions> options, TokenCredential credential)
    {
        this.secretClient = new SecretClient(options.Value.KeyVaultUri, credential);
    }

    public async Task<string?> GetSecret(string key)
    {
        var secretResponse = await secretClient.GetSecretAsync(key);
        return secretResponse?.Value?.Value;
    }

    public async Task SetSecret(string key, string value)
    {
        await secretClient.SetSecretAsync(key, value);
    }
}
