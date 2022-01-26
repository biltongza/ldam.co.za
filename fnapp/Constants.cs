namespace ldam.co.za.fnapp;

public static class Constants
{
    public static class Configuration
    {
        public static class Adobe
        {
            public const string CreativeCloudBaseUrl = "AdobeCreativeCloudBaseUrl";
            public const string AuthClientId = "AdobeAuthClientId";
            public const string AlbumIds = "AdobeAlbumIds";
            public const string SizesToSync = "AdobeSizesToSync";
            public const string RefreshTokenWindowMinutes = "AdobeRefreshTokenWindowMinutes";
        }

        public static class Azure
        {
            public const string CdnSubscriptionId = "AzureCdnSubscriptionId";
            public const string CdnResourceGroup = "AzureCdnResourceGroup";
            public const string CdnProfileName = "AzureCdnProfileName";
            public const string CdnEndpointName = "AzureCdnEndpointName";
        }
    }
}
