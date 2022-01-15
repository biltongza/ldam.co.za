using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.lib.Services;

public interface IStorageService
{
    Task Store(string name, Stream stream, string contentType = null);
    Task<Stream> Get(string name);
    Task DeleteBlobsStartingWith(string startsWith);
}

public class StorageService : IStorageService
{
    private readonly BlobContainerClient blobContainerClient;
    private readonly ILogger logger;
    public StorageService(IConfiguration configuration, ILogger<StorageService> logger)
    {
        this.logger = logger;
        var storageUri = configuration[Constants.Configuration.Azure.BlobStorageUri];
        var blobServiceClient = new BlobServiceClient(new Uri(storageUri), new ChainedTokenCredential(new ManagedIdentityCredential(), new AzureCliCredential()));
        this.blobContainerClient = blobServiceClient.GetBlobContainerClient(configuration[Constants.Configuration.Azure.BlobContainer]);
    }

    public async Task Store(string name, Stream stream, string contentType = null)
    {
        this.logger.LogInformation("Storing blob with name {name} and content type {contentType}", name, contentType);
        var blobClient = this.blobContainerClient.GetBlobClient(name);
        var blobHeaders = new BlobHttpHeaders { ContentType = contentType };
        await blobClient.UploadAsync(stream, options: new BlobUploadOptions { HttpHeaders = blobHeaders });
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

    public async Task DeleteBlobsStartingWith(string startsWith)
    {
        logger.LogInformation("Deleting blobs starting with {startsWith}", startsWith);
        var blobs = this.blobContainerClient.GetBlobsAsync(prefix: startsWith);
        await foreach (var blob in blobs)
        {
            logger.LogInformation("Deleting blob {name}", blob.Name);
            await this.blobContainerClient.DeleteBlobIfExistsAsync(blob.Name);
        }
    }
}
