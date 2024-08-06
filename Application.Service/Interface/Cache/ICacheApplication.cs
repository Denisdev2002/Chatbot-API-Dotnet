using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Service.Interface.Cache
{
    public interface ICacheApplication
    {
        Task SetCacheAsync<T>(string key, T value, TimeSpan expiration);
        Task<T?> GetCacheAsync<T>(string key);

    }
}
