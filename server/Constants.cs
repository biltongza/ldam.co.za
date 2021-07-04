namespace ldam.co.za.server
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
    }
}