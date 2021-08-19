using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za.lib.Services
{
    public interface IStorageService
    {
        Task Store(string name, Stream stream, bool overwrite = true);
        Task<Stream> Get(string name);
    }
    
    public class StorageService : IStorageService
    {
        private readonly BlobContainerClient blobContainerClient;
        public StorageService(IConfiguration configuration)
        {
            var storageUri = configuration[Constants.Configuration.Azure.BlobStorageUri];
            var blobServiceClient = new BlobServiceClient(new Uri(storageUri), new ChainedTokenCredential(new ManagedIdentityCredential(), new AzureCliCredential()));
            blobContainerClient = blobServiceClient.GetBlobContainerClient(configuration[Constants.Configuration.Azure.BlobContainer]);
        }

        public async Task Store(string name, Stream stream, bool overwrite = true)
        {
            var blobClient = blobContainerClient.GetBlobClient(name);
            await blobClient.UploadAsync(stream, overwrite: overwrite);
        }

        public async Task<Stream> Get(string name)
        {
            var blobClient = blobContainerClient.GetBlobClient(name);
            
            var exists = await blobClient.ExistsAsync();
            if(!exists.Value)
            {
                return Stream.Null;
            }

            var stream = await blobClient.OpenReadAsync();
            return stream;
        }
    }
}