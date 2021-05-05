using System.Threading.Tasks;

namespace FarmHub.Application.Services.Infrastructure
{
    public interface ICacheService
    {
        Task<string> GetCachedResponseAsync(string cacheKey);
        Task<T> GetCachedResponseAsync<T>(string cacheKey);
        Task CacheResponseAsync(string key, object response, int validitInMinutes);
        Task RefreshCachedResponseAsync(string key);
        Task DeleteCachedResponseAsync(string key);
    }
}