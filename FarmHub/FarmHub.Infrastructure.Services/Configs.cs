using System;
using Microsoft.Extensions.Caching.StackExchangeRedis;

namespace FarmHub.Domain.Services
{
    public class AzureStorageConfig
    {
        public string AccountKey { get; set; }
        public string AccountName { get; set; }
        public string BaseUrl { get; set; }
        public Uri BlobEndpoint { get; set; }
        public string Container { get; set; }
        public Uri QueueEndpoint { get; set; }
        public Uri TableEndpoint { get; set; }
    }

    public class AdyenConfig
    {
        public string ApiKey { get; set; }
        public string Environment { get; set; }
        public string MerchantAccount { get; set; }
    }

    public class CacheConfiguration
    {
        public int CartCacheTimeSpan { get; set; }
        public RedisCacheOptions RedisCacheOptions { get; set; }
    }

    public class GoogleRecaptchaConfig
    {
        public string SiteKey { get; set; }
        public string SecretKey { get; set; }
    }
}