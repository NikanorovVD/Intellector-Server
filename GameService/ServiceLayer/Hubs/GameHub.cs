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
        private readonly KafkaProducerBackgroundService<MoveDto> _kafkaProducer;
        private readonly GameRedisService _gameService;
        private readonly BoardService _boardService;
        private readonly IMapper _mapper;

        public GameHub(KafkaProducerBackgroundService<MoveDto> kafkaProducer, GameRedisService gameService, BoardService boardService, IMapper mapper)
        {
            _kafkaProducer = kafkaProducer;
            _gameService = gameService;
            _boardService = boardService;
            _mapper = mapper;
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
            if (!board.CheckIfMoveIsAvailable(move)) throw new HubException("Illegal move");

            // оправка хода клиентам
            await Clients.All.SendAsync(GameHubMethods.ReceiveMove, move, playerColor);

            // отправка хода в Kafka
            MoveDto moveDto = new()
            {
                Move = move,
                UserId = userId,
                MadeAt = DateTime.UtcNow
            };
            _kafkaProducer.EnqueueMessage(moveDto);

            // проверка является ли ход победным
            if (board.CheckIfMoveAreWinning(move))
            {
                // отправка результата игры в Kafka

                // отправка результата игры клиентам 
            }

            // переключение очередности хода
            await _gameService.SwitchTurnAsync(gameId);

            // изменение позиций фигур
            await _boardService.UpdateBoardWithMoveAsync(gameId, move);
        }
    }
}
