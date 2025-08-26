using DataLayer;
using Shared.Models;


namespace ServiceLayer.Services
{
    public class GameRedisService
    {
        private readonly RedisJsonStorageService<Game> _redisService;

        public GameRedisService(RedisJsonStorageService<Game> redisService)
        {
            _redisService = redisService;
        }

        public async Task<string> CreateAsync(Game game)
        {
            string id = Guid.NewGuid().ToString();
            await _redisService.WriteAsync(id, game);
            return id;
        }

        public async Task<Game> GetAsync(string id)
        {    
            return await _redisService.ReadAsync(id);
        }

        public async Task UpdateGameWithMove(string id)
        {            
            Game game = await _redisService.ReadAsync(id);

            game.Turn = game.Turn == PlayerColor.White ?
                PlayerColor.Black :
                PlayerColor.White;

            game.MoveCount++;

            await _redisService.WriteAsync(id,  game);
        }

        public async Task DeleteAsync(string id)
        {
            await _redisService.DeleteAsync(id);
        }
    }
}
