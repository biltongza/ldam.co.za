using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za.server
{
    public class CreativeCloudService : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string libraryId;
        private readonly string apiKey;
        private readonly string accessToken;

        public CreativeCloudService(IConfiguration configuration)
        {
            var ccAddress = configuration.GetValue<string>(Constants.Configuration.CCBaseUrl);
            this.libraryId = configuration.GetValue<string>(Constants.Configuration.CCLibraryId);
            this.apiKey = configuration.GetValue<string>(Constants.Configuration.CCApiKey);
            this.httpClient = new HttpClient
            {
                BaseAddress = new Uri(ccAddress),
            };
            this.httpClient.DefaultRequestHeaders.Add("x-api-key", this.apiKey);
        }

        public async Task<byte[]> GetImage()
        {
            return await this.httpClient.GetByteArrayAsync($"");
        }

        public void Dispose()
        {
            ((IDisposable)httpClient).Dispose();
        }
    }
}