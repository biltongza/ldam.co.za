namespace ldam.co.za.backend
{
    public static class Constants
    {
        public static class AdobeConfiguration
        {
            private const string BasePath = "Adobe";
            public const string CreativeCloudBaseUrl = BasePath + ":CreativeCloudBaseUrl";
            public const string LibraryId = BasePath + ":LibraryId";
            public static class Auth
            {
                private const string BasePath = AdobeConfiguration.BasePath + ":Auth";
                public const string ClientId = BasePath + ":ClientId";
                public const string ClientSecret = BasePath + ":ClientSecret";
            }

        }
        public static class AzureConfiguration
        {
            private const string BasePath = "Azure";
            public const string KeyVaultUri = BasePath + ":KeyVaultUri";
        }
    }
}