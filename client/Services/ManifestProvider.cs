using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ldam.co.za.lib;
using ldam.co.za.lib.Models;

namespace ldam.co.za.client.Services
{
    public interface IManifestProvider
    {
        Task<Manifest> Manifest { get; }
    }

    public class ManifestProvider : IManifestProvider
    {
        private AsyncLazy<Manifest> lazyManifest;

        public ManifestProvider(IHttpClientFactory httpClientFactory)
        {
            var cdnHttpClient = httpClientFactory.CreateClient(Constants.HttpClients.Cdn);
            lazyManifest = new AsyncLazy<Manifest>(() => cdnHttpClient.GetFromJsonAsync<Manifest>($"portfolio/manifest.json?t={System.DateTime.Now.Ticks}"));
        }

        public Task<Manifest> Manifest
        {
            get => lazyManifest.Value;
        }
    }
}