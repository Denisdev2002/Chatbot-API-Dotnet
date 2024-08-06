using Application.Service.Interface.Cache;
using Domain.Service.Services.Cache.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Application.Cache
{
    public class CacheApplication : ICacheApplication
    {
        private readonly ICacheService _cacheService;

        public CacheApplication(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }

        public async Task<T?> GetCacheAsync<T>(string key)
        {
            return await _cacheService.GetCacheAsync<T>(key);
        }

        public async Task SetCacheAsync<T>(string key, T value, TimeSpan expiration)
        {
            await _cacheService.SetCacheAsync(key, value, expiration);
        }
    }
}
