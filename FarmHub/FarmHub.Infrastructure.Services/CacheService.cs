using System;
using System.Threading.Tasks;
using FarmHub.Application.Services.Infrastructure;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace FarmHub.Domain.Services
{
    public class CacheService : ICacheService
    {
        private IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }    

        public async Task<string> GetCachedResponseAsync(string cacheKey)
        {
            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);

            return string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
        }

        public async Task<T> GetCachedResponseAsync<T>(string cacheKey)
        {
            var cachedResponse = await _distributedCache.GetStringAsync(cacheKey);
            var json = string.IsNullOrEmpty(cachedResponse) ? null : cachedResponse;
            return JsonConvert.DeserializeObject<T>(json);
        }

        public async Task CacheResponseAsync(string key, object response, int validityInMinutes)
        {
            if (response == null)
            {
                return;
            }

            var validity = new TimeSpan(0, validityInMinutes, 0);
            var serializedResponse = JsonConvert.SerializeObject(response);

             await _distributedCache.SetStringAsync(key, serializedResponse, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = validity
            });
        }
        
        public async Task RefreshCachedResponseAsync(string key)
        {
            await _distributedCache.RefreshAsync(key);
        }
        
        public async Task DeleteCachedResponseAsync(string key)
        {
            await _distributedCache.RemoveAsync(key);
        }
    }
}