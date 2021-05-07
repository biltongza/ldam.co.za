using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class LightroomClient : ILightroomClient, IDisposable
    {
        private HttpClient httpClient;
        private AccessTokenProvider accessTokenProvider;
        private string baseUrl;
        public LightroomClient(IHttpClientFactory httpClientFactory, AccessTokenProvider accessTokenProvider, string baseUrl, string apiKey)
        {
            this.accessTokenProvider = accessTokenProvider;
            httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X_API_KEY", apiKey);
            this.baseUrl = baseUrl;
        }

        protected HttpRequestMessage PrepareRequest(HttpMethod method, string resource)
        {
            var request = new HttpRequestMessage();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessTokenProvider.AccessToken);
            request.RequestUri = new Uri(new Uri(this.baseUrl), resource);
            request.Method = method;
            return request;
        }

        protected async Task<T> HandleResponse<T>(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
            return result;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async Task<CatalogResponse> GetCatalog()
        {
            var request = PrepareRequest(HttpMethod.Get, "/v2/catalog");
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<CatalogResponse>(response);
            return result;
        }
    }
}