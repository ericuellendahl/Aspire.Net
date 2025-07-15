using Aspire.Net.ApiService.Domain.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace Aspire.Net.ApiService.Infrastrutura.Repositories
{
    public class RedisCacheRepository : ICacheRepository
    {
        private readonly IConnectionMultiplexer _muxer;

        public RedisCacheRepository(IConnectionMultiplexer muxer)
        {
            _muxer = muxer;
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var db = _muxer.GetDatabase();
            var serializedValue = JsonSerializer.Serialize(value);
            await db.StringSetAsync(key, serializedValue, expiry);
        }

        public async Task<T> GetAsync<T>(string key)
        {
            var db = _muxer.GetDatabase();
            var serializedValue = await db.StringGetAsync(key);
            if (serializedValue.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(serializedValue);
        }

        public async Task<bool> IsHealthyAsync()
        {
            try
            {
                var db = _muxer.GetDatabase();
                await db.PingAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
