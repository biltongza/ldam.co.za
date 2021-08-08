using Microsoft.Extensions.Hosting;
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
                    services.AddTransient<ILightroomService, LightroomService>();
                    services.AddTransient<IStorageService, StorageService>();
                    services.AddTransient<SyncService>();
                })
                .Build();

            host.Run();
        }
    }
}