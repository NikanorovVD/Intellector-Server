using DataLayer;
using IntellectorLogic;
using Shared.Models;

namespace ServiceLayer.Services
{
    public class BoardService
    {
        private const string _boardIdPrefix = "board:";
        private readonly RedisJsonStorageService<Board> _redisService;

        public BoardService(RedisJsonStorageService<Board> redisService)
        {
            _redisService = redisService;
        }

        public async Task CreateBoardAsync(string id)
        {
            string key = GetBoardKey(id);
            Board board = Board.CreateWithStartPosition();

            await _redisService.WriteAsync(key, board);
        }

        public async Task<Board> GetBoardAsync(string id)
        {
            string key = GetBoardKey(id);
            return await _redisService.ReadAsync(key);
        }

        public async Task UpdateBoardWithMoveAsync(string id, Move move)
        {
            string key = GetBoardKey(id);
            Board board = await _redisService.ReadAsync(key);
            board.ExecuteMove(move);
            await _redisService.WriteAsync(key, board);
        }

        public async Task DeleteBoardAsync(string id)
        {
            string key = GetBoardKey(id);
            await _redisService.DeleteAsync(key);
        }

        private string GetBoardKey(string id)
        {
            return $"{_boardIdPrefix}{id}";
        }
    }
}
