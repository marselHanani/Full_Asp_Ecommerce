using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Ecommerce.Application.Service
{
    public class CacheService(IMemoryCache cache, ILogger<CacheService> logger)
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
                if (cache.TryGetValue(key, out var obj))
                {
                    if (obj is byte[] bytes)
                    {
                        var json = System.Text.Encoding.UTF8.GetString(bytes);
                        return JsonSerializer.Deserialize<T>(json, _jsonOptions);
                    }
                }
                return default;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error accessing MemoryCache for key: {Key}", key);
                return default;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var json = JsonSerializer.Serialize(value, _jsonOptions);
                var bytes = System.Text.Encoding.UTF8.GetBytes(json);

                var options = new MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = expiry ?? TimeSpan.FromMinutes(30)
                };

                cache.Set(key, bytes, options);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding to MemoryCache for key: {Key}", key);
                throw;
            }
        }

        public async Task RemoveAsync(string key)
        {
            try
            {
                cache.Remove(key);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error removing from MemoryCache for key: {Key}", key);
                throw;
            }
        }
    }
}
