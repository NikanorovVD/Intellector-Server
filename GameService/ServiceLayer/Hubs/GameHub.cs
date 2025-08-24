using AutoMapper;
using DataLayer;
using KafkaMessaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ServiceLayer.Services;
using Shared.Models;
using System.Security.Claims;


namespace ServiceLayer.Hubs
{
    [Authorize]
    public class GameHub: Hub
    {
        private readonly KafkaProducerBackgroundService<MoveDto> _kafkaProducer;
        private readonly GameRedisService _gameService;
        private readonly IMapper _mapper;

        public GameHub(KafkaProducerBackgroundService<MoveDto> kafkaProducer, GameRedisService gameService, IMapper mapper)
        {
            _kafkaProducer = kafkaProducer;
            _gameService = gameService;
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
            if(game.Turn != playerColor) throw new HubException("Not your turn");

            // проверка валидности хода

            // переключение очередности хода
            await _gameService.SwitchTurnAsync(gameId);

            // изменение позиций фигур в redis

            // отправка хода в Kafka
            MoveDto moveDto = new()
            {
                Move = move,
                UserId = userId,
                MadeAt = DateTime.UtcNow
            };
            _kafkaProducer.EnqueueMessage(moveDto);

            // оправка хода клиентам
            await Clients.All.SendAsync(GameHubMethods.ReceiveMove, move, playerColor);
        }
    }
}
