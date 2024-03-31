using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using ldam.co.za.fnapp;
using ldam.co.za.fnapp.Services;
using ldam.co.za.lib.Configuration;
using ldam.co.za.lib.Lightroom;
using ldam.co.za.lib.Services;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices((context, services) =>
    {
        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        services.AddMemoryCache();
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
        services.AddTransient<IWebPEncoderService, WebPEncoderService>();
        services.AddSingleton<TokenCredential>((_) =>
                new ChainedTokenCredential(
#if DEBUG
                    new AzureCliCredential(),
#endif
                    new ManagedIdentityCredential()
                ));
        services.AddSingleton<ArmClient>();
        services.AddTransient<ICdnService, CdnService>();

        var config = context.Configuration;
        services.Configure<FunctionAppAzureResourceOptions>(config.GetSection("Azure"));
        services.Configure<AzureResourceOptions>(config.GetSection("Azure"));
        services.Configure<FunctionAppLightroomOptions>(config.GetSection("Lightroom"));
        services.Configure<LightroomOptions>(config.GetSection("Lightroom"));
    })
    .Build();

host.Run();