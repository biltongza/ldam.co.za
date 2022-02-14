using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using ldam.co.za.fnapp.Services;
using ldam.co.za.lib.Configuration;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(ldam.co.za.fnapp.Startup))]

namespace ldam.co.za.fnapp;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var services = builder.Services;

        var config = builder.GetContext().Configuration;
        services.AddMemoryCache();
        services.AddOptions<FunctionAppAzureResourceOptions>("Azure");
        services.AddOptions<AzureResourceOptions>("Azure");
        services.AddOptions<FunctionAppLightroomOptions>("Lightroom");
        services.AddOptions<LightroomOptions>("Lightroom");
        services.AddTransient<ISecretService, SecretService>();

        services.AddTransient<IAccessTokenProvider, AccessTokenProvider>();
        services.AddHttpClient("lightroom")
            .ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
            {
                AllowAutoRedirect = true,
            })
            .RedactLoggedHeaders(new string[] { "Authorization", "X-API-KEY" });

        services.AddTransient<ILightroomClient, LightroomClient>();
        services.AddTransient<ILightroomService, LightroomService>();
        services.AddTransient<IStorageService, StorageService>();
        services.AddTransient<IClock, Clock>();
        services.AddTransient<RefreshTokenService>();
        services.AddTransient<SyncService>();
        services.AddTransient<ILightroomTokenService, LightroomTokenService>();
        services.AddTransient<IMetadataService, MetadataService>();
        services.AddSingleton<TokenCredential>((_) =>
                new ChainedTokenCredential(
#if DEBUG
                    new AzureCliCredential(),
#endif
                    new ManagedIdentityCredential()
                ));
        services.AddSingleton<ArmClient>();
        services.AddTransient<ICdnService, CdnService>();
        services.Configure<FunctionAppAzureResourceOptions>(config.GetSection("Azure"));
        services.Configure<AzureResourceOptions>(config.GetSection("Azure"));
        services.Configure<FunctionAppLightroomOptions>(config.GetSection("Lightroom"));
        services.Configure<LightroomOptions>(config.GetSection("Lightroom"));
    }
}
