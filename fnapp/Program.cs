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

namespace ldam.co.za.fnapp
{
    public class Program
    {
        public static void Main()
        {
            IConfigurationRefresher configurationRefresher = null;
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureAppConfiguration((hostBuilder, configurationBuilder) =>
                {
                    if (hostBuilder.HostingEnvironment.IsDevelopment())
                    {
                        configurationBuilder
                            .AddJsonFile(Path.Combine(hostBuilder.HostingEnvironment.ContentRootPath, $"appsettings.{hostBuilder.HostingEnvironment.EnvironmentName}.json"), optional: true, reloadOnChange: false);
                            //.AddUserSecrets(optional: true, reloadOnChange: false);
                    }

                    var configuration = hostBuilder.Configuration;
                    var applicationConfigurationEndpoint = configuration["APPLICATIONCONFIGURATION_ENDPOINT"];

                    if (!string.IsNullOrEmpty(applicationConfigurationEndpoint))
                    {
                        configurationBuilder.AddAzureAppConfiguration(appConfigOptions =>
                        {
                            var azureCredential = new DefaultAzureCredential(includeInteractiveCredentials: false);

                            appConfigOptions
                                .Connect(new Uri(applicationConfigurationEndpoint), azureCredential)
                                .ConfigureKeyVault(keyVaultOptions =>
                                {
                                    keyVaultOptions.SetCredential(azureCredential);
                                })
                                .ConfigureRefresh(refreshOptions =>
                                {
                                    refreshOptions.Register(key: "Application:ConfigurationVersion", label: LabelFilter.Null, refreshAll: true);
                                    refreshOptions.SetCacheExpiration(TimeSpan.FromMinutes(3));
                                });

                            configurationRefresher = appConfigOptions.GetRefresher();
                        });
                    }
                })
                .ConfigureServices((hostBuilder, services) =>
                {
                    if (configurationRefresher is not null)
                    {
                        services.AddSingleton(configurationRefresher);
                    }
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
                })
                .Build();

            host.Run();
        }
    }
}