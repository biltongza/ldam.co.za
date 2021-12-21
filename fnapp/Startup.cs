using System.Net.Http;
using ldam.co.za.fnapp.Services;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ldam.co.za.fnapp.Startup))]

namespace ldam.co.za.fnapp
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;

            var config = builder.GetContext().Configuration;
            services.AddMemoryCache();
            services.AddTransient<ISecretService, SecretService>((_) => new SecretService(config[lib.Constants.Configuration.Azure.KeyVaultUri]));

            services.AddTransient<IAccessTokenProvider, AccessTokenProvider>();
            services.AddHttpClient("lightroom")
                .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
                {
                    AllowAutoRedirect = true,
                })
                .RedactLoggedHeaders(new string[] {"Authorization", "X-API-KEY"});

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
        }
    }
}