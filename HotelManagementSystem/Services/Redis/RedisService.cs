using HotelManagementSystem.Interfaces.Redis;
using StackExchange.Redis;
using System.Text.Json;

namespace HotelManagementSystem.Services.Redis
{
    public class RedisService : IRedisService
    {
        private readonly IDatabase _database;

        public RedisService(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task SetAsync<T>(
            string key,
            T value,
            TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);

            await _database.StringSetAsync(
                key,
                json,
                expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var value = await _database.StringGetAsync(key);

            if (value.IsNullOrEmpty)
                return default;

            return JsonSerializer.Deserialize<T>(value.ToString()!);
        }

        public async Task RemoveAsync(string key)
        {
            await _database.KeyDeleteAsync(key);
        }

        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(key);
        }

        public async Task<Y>
    }
}