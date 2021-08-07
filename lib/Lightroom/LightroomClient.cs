using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ldam.co.za.lib.Lightroom
{
    public class LightroomClient : ILightroomClient, IDisposable
    {
        private HttpClient httpClient;
        private IAccessTokenProvider accessTokenProvider;
        private string baseUrl;
        private static readonly Regex WhileRegex = new Regex(@"^while\s*\(\s*1\s*\)\s*{\s*}\s*", RegexOptions.Compiled);
        public LightroomClient(IHttpClientFactory httpClientFactory, IAccessTokenProvider accessTokenProvider, string baseUrl, string apiKey)
        {
            this.accessTokenProvider = accessTokenProvider;
            httpClient = httpClientFactory.CreateClient("lightroom");
            httpClient.DefaultRequestHeaders.Add("X-API-KEY", apiKey);
            this.baseUrl = baseUrl;
        }

        protected async Task<HttpRequestMessage> PrepareRequest(HttpMethod method, string resource)
        {
            var request = new HttpRequestMessage();
            var accessToken = await accessTokenProvider.GetAccessToken();
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
            request.RequestUri = new Uri(new Uri(this.baseUrl), resource);
            request.Method = method;
            return request;
        }

        protected async Task<T> HandleResponse<T>(HttpResponseMessage response, bool stripWhile = true)
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            if (stripWhile)
            {
                content = WhileRegex.Replace(content, "");
            }
            var result = JsonSerializer.Deserialize<T>(content);
            return result;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        public async Task<CatalogResponse> GetCatalog()
        {
            var request = await PrepareRequest(HttpMethod.Get, "/v2/catalog");
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<CatalogResponse>(response);
            return result;
        }

        public async Task<HealthResponse> GetHealth()
        {
            var request = await PrepareRequest(HttpMethod.Get, "/v2/health");
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
            var request = await PrepareRequest(HttpMethod.Get, builder.ToString());
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
            builder.Append("/assets?embed=asset;spaces;user&subtype=image");
            if (!string.IsNullOrWhiteSpace(after))
            {
                builder.Append("captured_after=");
                builder.Append(after);
            }
            var request = await PrepareRequest(HttpMethod.Get, builder.ToString());
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<AlbumAssetResponse>(response);
            return result;
        }

        public async Task<AssetResponse> GetAsset(string catalogId, string assetId)
        {
            var request = await PrepareRequest(HttpMethod.Get, $"v2/catalogs/{catalogId}/assets/{assetId}");
            var response = await httpClient.SendAsync(request);
            var result = await HandleResponse<AssetResponse>(response);
            return result;
        }

        public async Task<Stream> GetImageStream(string catalogId, string assetId, string size)
        {
            var asset = await GetAsset(catalogId, assetId);
            
            var href = asset.Links[$"rels/rendition_type/{size}"]?.Href;
            if(string.IsNullOrWhiteSpace(href))
            {
                throw new InvalidOperationException($"Size {size} is not available");
            }

            var builder = new StringBuilder();
            builder.Append("/v2/catalogs/");
            builder.Append(catalogId);
            builder.Append("/");
            builder.Append(href);

            var request = await PrepareRequest(HttpMethod.Get, builder.ToString());
            var response = await httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStreamAsync();
        }
    }
}