using Enyim.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.Application.Service
{
    public class CacheService(IDistributedCache cache, ILogger<CacheService> logger)
    {
        private readonly JsonSerializerOptions _jsonOptions = new()
        {
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve,
            MaxDepth = 32,
            WriteIndented = true
        };

        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var bytes = await cache.GetAsync(key);
                if (bytes == null) return default;
                var json = System.Text.Encoding.UTF8.GetString(bytes);
                return JsonSerializer.Deserialize<T>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error accessing Memcached for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(value, _jsonOptions);
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);

                var options = new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
                };

                await cache.SetAsync(key, bytes, options);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding to Memcached for key: {Key}", key);
                throw;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                await cache.RemoveAsync(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing from Memcached for key: {Key}", key);
                throw;
            }
        }
    }
}
