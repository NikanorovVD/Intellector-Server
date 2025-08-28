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
        private readonly BoardService _boardService;
        private readonly IMapper _mapper;
        private readonly KafkaProducerBackgroundService<StartGameMessage> _startKafkaProducer;

        public GameStartKafkaConsumerService(IServiceScopeFactory serviceScopeFactory, KafkaListenerService<CreateGameRequest> kafkaListenerService)
            : base(kafkaListenerService)
        {
            var scope = serviceScopeFactory.CreateScope();
            _hubContext = scope.ServiceProvider.GetRequiredService<IHubContext<GameHub>>();
            _mapper = scope.ServiceProvider.GetRequiredService<IMapper>();
            _gameService = scope.ServiceProvider.GetRequiredService<GameRedisService>();
            _boardService = scope.ServiceProvider.GetRequiredService<BoardService>();
            _startKafkaProducer = scope.ServiceProvider.GetRequiredService<KafkaProducerBackgroundService<StartGameMessage>>();
        }

        protected override async Task ProcessMessageAsync(CreateGameRequest gameRequest)
        {
            // создание игры в redis
            Game game = _mapper.Map<Game>(gameRequest);
            game.Turn = PlayerColor.White;

            string id = await _gameService.CreateAsync(game);
            await _boardService.CreateBoardAsync(id);

            // отправка сообщения в Kafka
            StartGameMessage startGameMessage = _mapper.Map<StartGameMessage>(gameRequest);
            startGameMessage.GameId = id;
            _startKafkaProducer.EnqueueMessage(startGameMessage);

            // отправка сообщения клиентам
            await _hubContext.Clients.Users(gameRequest.WhitePlayerId, gameRequest.BlackPlayerId).SendAsync(GameHubMethods.ReceiveStartGame, startGameMessage);
        }
    }
}
