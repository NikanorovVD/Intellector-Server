using StackExchange.Redis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
namespace DataLayer
{
    public class RedisJsonStorageService<TValue>
    {
        private readonly IDatabase _redisClient;

        public RedisJsonStorageService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisClient = connectionMultiplexer.GetDatabase();
        }

        public async Task WriteAsync(string key, TValue value)
        {
            string json = JsonConvert.SerializeObject(value);
            await _redisClient.StringSetAsync(key, json);
        }

        public async Task<TValue> ReadAsync(string key)
        {
            string json = await _redisClient.StringGetAsync(key);
            return JsonConvert.DeserializeObject<TValue>(json);
        }

        public async Task DeleteAsync(string key)
        {
            await _redisClient.KeyDeleteAsync(key);
        }
    }
}
