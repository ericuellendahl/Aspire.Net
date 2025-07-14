using Aspire.Net.ApiService.Domain.Interfaces;

namespace Aspire.Net.ApiService.Application.Services
{
    public class CacheService
    {
        private readonly ICacheRepository _cacheRepository;

        public CacheService(ICacheRepository cacheRepository)
        {
            _cacheRepository = cacheRepository;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            await _cacheRepository.SetAsync(key, value, expiry);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            return await _cacheRepository.GetAsync<T>(key);
        }
    }
}
