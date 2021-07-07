using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class LightroomClient : ILightroomClient, IDisposable
    {
        private HttpClient httpClient;
        private AccessTokenProvider accessTokenProvider;
        private string baseUrl;
        //"while (1) {}\n"
        private static readonly Regex WhileRegex = new Regex(@"^while\s*\(\s*1\s*\)\s*{\s*}\s*", RegexOptions.Compiled);
        public LightroomClient(IHttpClientFactory httpClientFactory, AccessTokenProvider accessTokenProvider, string baseUrl, string apiKey)
        {
            this.accessTokenProvider = accessTokenProvider;
            httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
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

        protected async Task<T> HandleResponse<T>(HttpResponseMessage response, bool stripWhile = true)
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if(stripWhile)
            {
                content = WhileRegex.Replace(content, "");
            }
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

        public async Task<object> GetHealth()
        {
            var request = PrepareRequest(HttpMethod.Get, "/v2/health");
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<object>(response);
            return result;
        }
    }
}