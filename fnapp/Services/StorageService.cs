using System;
using System.IO;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ldam.co.za.fnapp.Services
{
    public class StorageService
    {
        private readonly BlobContainerClient blobContainerClient;
        public StorageService(IConfiguration configuration)
        {
            var storageUri = configuration[Constants.Configuration.Azure.BlobStorageUri];
            var blobServiceClient = new BlobServiceClient(new Uri(storageUri));
            blobContainerClient = blobServiceClient.GetBlobContainerClient(configuration[Constants.Configuration.Azure.BlobContainer]);
        }

        public async Task Store(string name, Stream stream)
        {
            var blobClient = blobContainerClient.GetBlobClient(name);
            await blobClient.UploadAsync(stream);
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