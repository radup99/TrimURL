using Microsoft.Extensions.Caching.Memory;
using TrimUrlApi.Controllers;

namespace TrimUrlApi.Services
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly ILogger<MemoryCacheService> _logger;

        public MemoryCacheService(IMemoryCache cache, ILogger<MemoryCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            _cache.TryGetValue(key, out T? value);
            return value;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null)
        {
            var options = new MemoryCacheEntryOptions();

            if (expiration.HasValue)
            {
                options.AbsoluteExpirationRelativeToNow = expiration;
            }

            _cache.Set(key, value, options);
        }

        public async Task RemoveAsync(string key)
        {
            _cache.Remove(key);
        }
    }
}
