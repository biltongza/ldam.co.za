using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za.lib.Services
{
    public class SecretService : ISecretService
    {
        private readonly SecretClient secretClient;
        public SecretService(string keyVaultUri)
        {
            this.secretClient = new SecretClient(new Uri(keyVaultUri), new DefaultAzureCredential());
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