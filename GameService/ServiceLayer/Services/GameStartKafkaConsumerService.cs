using AutoMapper;
using DataLayer;
using KafkaMessaging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using ServiceLayer.Hubs;
using ServiceLayer.Models;
using Shared.Models;


namespace ServiceLayer.Services
{
    public class GameStartKafkaConsumerService : KafkaConsumerBackgroundService<CreateGameRequest>
    {
        private readonly IHubContext<GameHub> _hubContext;
        private readonly GameRedisService _gameService;
        private readonly IMapper _mapper;

        public GameStartKafkaConsumerService(IServiceScopeFactory serviceScopeFactory, KafkaListenerService<CreateGameRequest> kafkaListenerService)
            : base(kafkaListenerService)
        {
            var scope = serviceScopeFactory.CreateScope();
            _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<GameHub>>();
            _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            _gameService = scope.ServiceProvider.GetRequiredService<GameRedisService>();
        }

        protected override async Task ProcessMessageAsync(CreateGameRequest gameRequest)
        {
            // создать игру в redis
            Game game = _mapper.Map<Game>(gameRequest);
            game.Turn = PlayerColor.White;
            string id = await _gameService.CreateAsync(game);

            // отправить сообщения клиентам
            StartGameMessage gameInfo = _mapper.Map<StartGameMessage>(gameRequest);
            gameInfo.GameId = id;

            await _hubContext.Clients.Users(gameRequest.WhitePlayerId, gameRequest.BlackPlayerId).SendAsync(GameHubMethods.ReceiveStartGame, gameInfo);
        }
    }
}
