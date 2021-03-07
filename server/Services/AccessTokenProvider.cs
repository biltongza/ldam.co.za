using System;
using System.Net.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace ldam.co.za.server
{
    public class AccessTokenProvider
    {
        private readonly IMemoryCache memoryCache;
        private readonly IConfiguration configuration;

        // public string AccessToken
        // {
        //     get
        //     {
                
        //     }
        // }
        public AccessTokenProvider(IMemoryCache memoryCache, IConfiguration configuration)
        {
            this.memoryCache = memoryCache;
            this.configuration = configuration;
            
        }

        // private string GetAccessToken()
        // {
        //     using(var httpClient = new HttpClient())
        //     {
        //         httpClient.BaseAddress = new Uri(configuration.GetValue<string>(Constants.Configuration.CCBaseUrl));
                
        //     }
        // }
    }
}