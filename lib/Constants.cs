namespace ldam.co.za.lib;
public static class Constants
{
    public static class KeyVault
    {
        public const string LightroomAccessToken = "LightroomAccessToken";
        public const string LightroomRefreshToken = "LightroomRefreshToken";
        public const string LightroomClientSecret = "LightroomClientSecret";
    }
    public static class Configuration
    {
        public static class Azure
        {
            public const string KeyVaultUri = "AzureKeyVaultUri";
            public const string BlobStorageUri = "AzureBlobStorageUri";
            public const string BlobContainer = "AzureBlobContainer";
        }
    }
}
