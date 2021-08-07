namespace ldam.co.za.fnapp
{
    public static class Constants
    {
        public static class Configuration
        {
            public static class Azure
            {
                public const string KeyVaultUri = "AzureKeyVaultUri";
                public const string BlobStorageUri = "AzureBlobStorageUri";
                public const string BlobContainer = "AzureBlobContainer";
            }
            public static class Adobe
            {
                public const string CreativeCloudBaseUrl = "AdobeCreativeCloudBaseUrl";
                public const string AuthClientId = "AdobeAuthClientId";
                public const string AlbumIds = "AdobeAlbumIds";
                public const string SizesToSync = "AdobeSizesToSync";
            }
        }
    }
}