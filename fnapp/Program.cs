using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Azure.Functions.Worker.Configuration;
using System.IO;
using Azure.Identity;
using System;
using Microsoft.Extensions.Configuration.AzureAppConfiguration;
using Microsoft.Extensions.DependencyInjection;
using ldam.co.za.lib.Services;
using ldam.co.za.lib.Lightroom;
using System.Net.Http;
using ldam.co.za.fnapp.Services;

namespace ldam.co.za.fnapp
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((hostBuilder, services) =>
                {
                    var config = hostBuilder.Configuration;
                    services.AddTransient<ISecretService, SecretService>((_) => new SecretService(config[Constants.Configuration.Azure.KeyVaultUri]));

                    services.AddTransient<IAccessTokenProvider, AccessTokenProvider>();
                    services.AddHttpClient("lightroom")
                        .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                        {
                            AllowAutoRedirect = true,
                        });

                    services.AddTransient<ILightroomClient, LightroomClient>((svp) =>
                    {
                        return new LightroomClient(
                            svp.GetRequiredService<IHttpClientFactory>(),
                            svp.GetRequiredService<IAccessTokenProvider>(),
                            config[Constants.Configuration.Adobe.CreativeCloudBaseUrl],
                            config[Constants.Configuration.Adobe.AuthClientId]
                        );
                    });
                    services.AddTransient<LightroomService>();
                    services.AddTransient<StorageService>();
                })
                .Build();

            host.Run();
        }
    }
}