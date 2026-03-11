using System;
using System.Threading.Tasks;

namespace ProductService.Domain.Interfaces
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan expiration);
        Task RemoveAsync(string key);
        Task RemoveByPatternAsync(string pattern);
    }
}
