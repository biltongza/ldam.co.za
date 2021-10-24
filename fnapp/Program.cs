using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using ldam.co.za.lib.Services;
using ldam.co.za.lib.Lightroom;
using System.Net.Http;
using ldam.co.za.fnapp.Services;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility;

namespace ldam.co.za.fnapp
{
    public class Program
    {
        public static async Task Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices((hostBuilder, services) =>
                {
                    var config = hostBuilder.Configuration;
                    services.AddApplicationInsightsTelemetryWorkerService();
                    services.AddTransient<ISecretService, SecretService>((_) => new SecretService(config[lib.Constants.Configuration.Azure.KeyVaultUri]));

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
                    services.AddTransient<IClock, Clock>();
                    services.AddTransient<RefreshTokenService>();
                    services.AddTransient<SyncService>();
                    services.AddTransient<ILightroomTokenService, LightroomTokenService>();
                    services.AddTransient<IMetadataService, MetadataService>();
                })
                .Build();

            await host.RunAsync();
        }
    }
}