using System.Net.Http.Json;
using ldam.co.za.lib.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

public interface ICdnService
{
    Task ClearCache(CancellationToken cancellationToken = default);
}

public class CdnService : ICdnService
{
    private readonly HttpClient httpClient;
    private readonly CdnOptions cdnOptions;
    private readonly ILogger logger;
    public CdnService(
        IOptions<CdnOptions> options,
        IHttpClientFactory httpClientFactory,
        ISecretService secretService,
        ILogger<CdnService> logger)
    {
        this.httpClient = httpClientFactory.CreateClient("cloudflare");
        httpClient.BaseAddress = new Uri("https://api.cloudflare.com/");
        var apiKey = secretService.GetSecret("CloudflareApiKey").GetAwaiter().GetResult(); // naughty
        httpClient.DefaultRequestHeaders.Add("X-Auth-Key", apiKey);
        httpClient.DefaultRequestHeaders.Add("X-Auth-Email", options.Value.Email);
        this.cdnOptions = options.Value;
        this.logger = logger;
    }

    public async Task ClearCache(CancellationToken cancellationToken = default)
    {
        var body = new { purge_everything = true };
        logger.LogInformation("Purging {CloudflareZone}", cdnOptions.ZoneId);
        var res = await httpClient.PostAsJsonAsync($"/client/v4/zones/{cdnOptions.ZoneId}/purge_cache", body, cancellationToken);
        var resBody = await res.Content.ReadAsStringAsync(cancellationToken);
        logger.LogInformation("Status = {Status} Body = {Body}", res.StatusCode, resBody);
        res.EnsureSuccessStatusCode();
    }
}