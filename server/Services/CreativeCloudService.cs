using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za.server
{
    public class CreativeCloudService : IDisposable
    {
        private readonly HttpClient httpClient;
        private readonly string libraryId;
        private readonly string apiKey;
        private readonly AccessTokenProvider accessTokenProvider;

        public CreativeCloudService(IConfiguration configuration, AccessTokenProvider accessTokenProvider)
        {
            var ccAddress = configuration.GetValue<string>(Constants.AdobeConfiguration.CreativeCloudBaseUrl);
            this.libraryId = configuration.GetValue<string>(Constants.AdobeConfiguration.LibraryId);
            this.apiKey = configuration.GetValue<string>(Constants.AdobeConfiguration.Auth.ClientId);
            this.httpClient = new HttpClient()
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