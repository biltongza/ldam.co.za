using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using ldam.co.za.lib.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ldam.co.za.lib.Services;

public interface IStorageService
{
    Task Store(string name, Stream stream, string? contentType = null, CancellationToken cancellationToken = default);
    Task<Stream> Get(string name);
    Task DeleteBlobsStartingWith(string startsWith, CancellationToken cancellationToken = default);
}

public class StorageService : IStorageService
{
    private readonly BlobContainerClient blobContainerClient;
    private readonly ILogger logger;
    public StorageService(IOptionsSnapshot<AzureResourceOptions> options, TokenCredential tokenCredential, ILogger<StorageService> logger)
    {
        this.logger = logger;
        var blobServiceClient = new BlobServiceClient(options.Value.BlobStorageUri, tokenCredential);
        this.blobContainerClient = blobServiceClient.GetBlobContainerClient(options.Value.BlobContainer);
    }

    public async Task Store(string name, Stream stream, string? contentType = null, CancellationToken cancellationToken = default)
    {
        this.logger.LogInformation("Storing blob with name {name} and content type {contentType}", name, contentType);
        var blobClient = this.blobContainerClient.GetBlobClient(name);
        var blobHeaders = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(stream, options: new BlobUploadOptions { HttpHeaders = blobHeaders }, cancellationToken);
    }

    public async Task<Stream> Get(string name)
    {
        this.logger.LogInformation("Getting blob with name {name}", name);
        var blobClient = this.blobContainerClient.GetBlobClient(name);

        var exists = await blobClient.ExistsAsync();
        if (!exists.Value)
        {
            this.logger.LogInformation("Blob with name {name} does not exist", name);
            return Stream.Null;
        }

        this.logger.LogInformation("Got stream for blob with name {name}", name);
        var stream = await blobClient.OpenReadAsync();
        return stream;
    }

    public async Task DeleteBlobsStartingWith(string startsWith, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Deleting blobs starting with {startsWith}", startsWith);
        var blobs = this.blobContainerClient.GetBlobsAsync(new GetBlobsOptions { Prefix = startsWith }, cancellationToken: cancellationToken);
        await foreach (var blob in blobs)
        {
            logger.LogInformation("Deleting blob {name}", blob.Name);
            await this.blobContainerClient.DeleteBlobIfExistsAsync(blob.Name, cancellationToken: cancellationToken);
        }
    }
}
