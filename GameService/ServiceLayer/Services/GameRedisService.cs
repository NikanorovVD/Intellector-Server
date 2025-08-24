using DataLayer;
using Shared.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace ServiceLayer.Services
{
    public class GameRedisService
    {
        private readonly IDatabase _redisClient;
        public GameRedisService(IConnectionMultiplexer connectionMultiplexer)
        {
            _redisClient = connectionMultiplexer.GetDatabase();
        }

        public async Task<string> CreateAsync(Game game)
        {
            string id = Guid.NewGuid().ToString();
            string json = JsonSerializer.Serialize(game);
            await _redisClient.StringSetAsync(id, json);
            return id;
        }

        public async Task<Game> GetAsync(string id)
        {
            string json = await _redisClient.StringGetAsync(id);
            return JsonSerializer.Deserialize<Game>(json);
        }

        public async Task SwitchTurnAsync(string id)
        {
            string json = await _redisClient.StringGetAsync(id);
            Game game = JsonSerializer.Deserialize<Game>(json);
            game.Turn = game.Turn == PlayerColor.White ?
                PlayerColor.Black :
                PlayerColor.White;

            json = JsonSerializer.Serialize(game);
            await _redisClient.StringSetAsync(id, json);
        }

        public async Task DeleteAsync(string id)
        {

        }
    }
}
