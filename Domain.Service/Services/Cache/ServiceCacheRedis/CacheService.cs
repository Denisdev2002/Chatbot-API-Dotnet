using Domain.Service.Services.Cache.Interface;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Domain.Service.Services.Cache.ServiceCacheRedis
{
    public class CacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan expiration)
        {
            var jsonData = JsonSerializer.Serialize(value);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };
            await _distributedCache.SetStringAsync(key, jsonData, options);
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            var jsonData = await _distributedCache.GetStringAsync(key);
            if (jsonData is null)
            {
                return default;
            }
            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
