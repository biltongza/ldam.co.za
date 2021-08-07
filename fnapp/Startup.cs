using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Azure.Security.KeyVault.Secrets;
using ldam.co.za.fnapp.Services;
using Microsoft.Extensions.Configuration;

[assembly: WebJobsStartup(typeof(ldam.co.za.fnapp.Startup))]

namespace ldam.co.za.fnapp
{
    public class Startup : IWebJobsStartup
    {

        public void Configure(IWebJobsBuilder builder)
        {
            var config = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            builder.Services.AddSingleton<IConfiguration>(config); 
            builder.Services.AddTransient<ISecretService, SecretService>();
        }
    }
}