using AutoMapper;
using DataLayer;
using IntellectorLogic;
using KafkaMessaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServiceLayer.Services;
using Shared.Models;
using System.Security.Claims;


namespace ServiceLayer.Hubs
{
    [Authorize]
    public class GameHub : Hub
    {
        private readonly KafkaProducerBackgroundService<MoveDto> _kafkaMoveProducer;
        private readonly KafkaProducerBackgroundService<GameResultDto> _kafkaResultProducer;
        private readonly GameRedisService _gameService;
        private readonly BoardService _boardService;

        public GameHub(KafkaProducerBackgroundService<MoveDto> kafkaMoveProducer, KafkaProducerBackgroundService<GameResultDto> kafkaResultProducer, GameRedisService gameService, BoardService boardService)
        {
            _kafkaMoveProducer = kafkaMoveProducer;
            _kafkaResultProducer = kafkaResultProducer;
            _gameService = gameService;
            _boardService = boardService;
        }

        public async Task SendMove(string gameId, Move move)
        {
            string userId = Context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            // загрузка данных
            Game game = await _gameService.GetAsync(gameId);

            string idWhite = game.WhitePlayerId;
            string idBlack = game.BlackPlayerId;

            PlayerColor playerColor;
            if (userId == idWhite) playerColor = PlayerColor.White;
            else if (userId == idBlack) playerColor = PlayerColor.Black;
            else throw new Exception();

            // авторизация
            if (userId != idWhite && userId != idBlack) throw new HubException("Access denied");

            // проверка очередности хода
            if (game.Turn != playerColor) throw new HubException("Not your turn");

            // проверка валидности хода
            Board board = await _boardService.GetBoardAsync(gameId);
            if (!board.CheckIfMoveIsAvailable(move, playerColor)) throw new HubException("Illegal move");

            // оправка хода клиентам
            await Clients.Users(idBlack, idWhite).SendAsync(GameHubMethods.ReceiveMove, move, playerColor);

            // отправка хода в Kafka
            MoveDto moveDto = new()
            {
                Move = move,
                GameId = gameId,
                Number = game.MoveCount,
                UserId = userId,
                MadeAt = DateTime.UtcNow
            };
            _kafkaMoveProducer.EnqueueMessage(moveDto);

            // проверка является ли ход победным
            if (board.CheckIfMoveAreWinning(move, out GameResultReason? reason))
            {
                GameResult gameResult = (userId == idWhite) ? GameResult.WhiteWins : GameResult.BlackWins;
                GameResultDto resultMessage = new(gameResult, reason.Value);

                // отправка результата игры в Kafka
                _kafkaResultProducer.EnqueueMessage(resultMessage);

                // отправка результата игры клиентам 
                await Clients.Users(idBlack, idWhite).SendAsync(GameHubMethods.ReceiveGameResult, resultMessage);
            }

            // переключение очередности хода и увеличение счетчика ходов
            await _gameService.UpdateGameWithMove(gameId);

            // изменение позиций фигур
            await _boardService.UpdateBoardWithMoveAsync(gameId, move);
        }
    }
}
