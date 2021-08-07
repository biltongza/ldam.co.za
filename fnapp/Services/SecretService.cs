using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za.fnapp.Services
{
    public class SecretService : ISecretService
    {
        private readonly SecretClient secretClient;
        public SecretService(IConfiguration configuration)
        {
            this.secretClient = new SecretClient(new Uri(configuration.GetValue<string>("KeyVaultUri")), new DefaultAzureCredential());
        }

        public string GetSecret(string key)
        {
            return this.secretClient.GetSecret(key)?.Value?.Value;
        }

        public void SetSecret(string key, string value)
        {
            this.secretClient.SetSecret(key, value);
        }
    }
}