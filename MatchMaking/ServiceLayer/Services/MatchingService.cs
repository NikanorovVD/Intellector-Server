using KafkaMessaging;
using Microsoft.AspNetCore.SignalR;
using ServiceLayer.Models;
using Shared.Models;

namespace ServiceLayer.Services
{
    public class MatchingService
    {
        private readonly KafkaProducerBackgroundService<GameInfo> _kafkaProducer;

        public MatchingService(KafkaProducerBackgroundService<GameInfo> kafkaProducer)
        {
            _kafkaProducer = kafkaProducer;
        }

        public void CreateGame(LobbyDto lobby, string userJoiningId)
        {
            (string whiteId, string blackId) = AssignColors(lobby, userJoiningId);
            GameInfo gameInfo = new GameInfo()
            {
                BlackPlayerId = blackId,
                WhitePlayerId = whiteId,
                Rating = lobby.Rating,
                TimeControl = lobby.TimeControl
            };

            _kafkaProducer.EnqueueMessage(gameInfo);
        }

        private (string whiteId, string blackId) AssignColors(LobbyDto lobby, string userJoiningId)
        {
            return lobby.ColorChoice switch
            {
                ColorChoice.White => (lobby.OwnerId, userJoiningId),
                ColorChoice.Black => (userJoiningId, lobby.OwnerId),
                ColorChoice.Random => new Random().Next() % 2 == 0 ? (lobby.OwnerId, userJoiningId) : (userJoiningId, lobby.OwnerId)
            };
        }
    }
}
