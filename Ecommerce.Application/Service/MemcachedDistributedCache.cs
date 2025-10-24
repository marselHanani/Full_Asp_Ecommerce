using Enyim.Caching;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Ecommerce.Application.Service
{
    public class MemcachedDistributedCache(IMemcachedClient client) : IDistributedCache
    {
        public byte[]? Get(string key)
        {
            var result = client.Get<string>(key);
            return result == null ? null : Encoding.UTF8.GetBytes(result);
        }

        public async Task<byte[]?> GetAsync(string key, CancellationToken token = default)
        {
            var result = await client.GetAsync<string>(key);
            return result == null ? null : Encoding.UTF8.GetBytes(result.Value);
        }

        public void Refresh(string key) { }

        public Task RefreshAsync(string key, CancellationToken token = default) => Task.CompletedTask;

        public void Remove(string key)
        {
            client.Remove(key);
        }

        public Task RemoveAsync(string key, CancellationToken token = default)
        {
            return client.RemoveAsync(key);
        }

        public void Set(string key, byte[] value, DistributedCacheEntryOptions options)
        {
            var stringValue = Encoding.UTF8.GetString(value);
            var expiration = options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(30);
            client.Set(key, stringValue, expiration);
        }

        public Task SetAsync(string key, byte[] value, DistributedCacheEntryOptions options, CancellationToken token = default)
        {
            var stringValue = Encoding.UTF8.GetString(value);
            var expiration = options.AbsoluteExpirationRelativeToNow ?? TimeSpan.FromMinutes(30);
            return client.SetAsync(key, stringValue, expiration);
        }
    }
}
