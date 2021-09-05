using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BlazorApplicationInsights;
using ldam.co.za.client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("#app");
            builder.Services.AddHttpClient(Constants.HttpClients.Base, (client) => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress));
            builder.Services.AddHttpClient(Constants.HttpClients.Cdn, (sp, client) => 
            {
                var config = sp.GetRequiredService<IConfiguration>();
                client.BaseAddress = new Uri(config[Constants.Configuration.StorageRoot]);
            });
            builder.Services.AddSingleton<IManifestProvider, ManifestProvider>();
            builder.Services.AddBlazorApplicationInsights();

            await builder.Build().RunAsync();
        }
    }
}
