using System;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using ldam.co.za.server.Services;

namespace ldam.co.za.server.Clients.Lightroom
{
    public class LightroomClient : ILightroomClient, IDisposable
    {
        private HttpClient httpClient;
        private AccessTokenProvider accessTokenProvider;
        private string baseUrl;
        private static readonly Regex WhileRegex = new Regex(@"^while\s*\(\s*1\s*\)\s*{\s*}\s*", RegexOptions.Compiled);
        public LightroomClient(IHttpClientFactory httpClientFactory, AccessTokenProvider accessTokenProvider, string baseUrl, string apiKey)
        {
            this.accessTokenProvider = accessTokenProvider;
            httpClient = httpClientFactory.CreateClient("lightroom");
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

        protected async Task<T> HandleResponse<T>(HttpResponseMessage response, bool stripWhile = true, bool )
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (stripWhile)
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

        public async Task<HealthResponse> GetHealth()
        {
            var request = PrepareRequest(HttpMethod.Get, "/v2/health");
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<HealthResponse>(response);
            return result;
        }

        public async Task<AlbumsResponse> GetAlbums(string catalogId, string after = null)
        {
            var builder = new StringBuilder();
            builder.Append("/v2/catalogs/");
            builder.Append(catalogId);
            builder.Append("/albums");
            if (!string.IsNullOrWhiteSpace(after))
            {
                builder.Append("?name_after=");
                builder.Append(after);
            }
            var request = PrepareRequest(HttpMethod.Get, builder.ToString());
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<AlbumsResponse>(response);
            return result;
        }

        public async Task<AlbumAssetResponse> GetAlbumAssets(string catalogId, string albumId, string after = null)
        {
            var builder = new StringBuilder();
            builder.Append("/v2/catalogs/");
            builder.Append(catalogId);
            builder.Append("/albums/");
            builder.Append(albumId);
            builder.Append("/assets");
            if (!string.IsNullOrWhiteSpace(after))
            {
                builder.Append("?captured_after=");
                builder.Append(after);
            }
            var request = PrepareRequest(HttpMethod.Get, builder.ToString());
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<AlbumAssetResponse>(response);
            return result;
        }

        public async Task<AssetResponse> GetAsset(string catalogId, string assetId)
        {
            var request = PrepareRequest(HttpMethod.Get, $"v2/catalogs/{catalogId}/assets/{assetId}");
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<AssetResponse>(response);
            return result;
        }
    }
}