using Shared.Models;

namespace ServiceLayer.Models
{
    public class StartGameMessage
    {
        public string GameId { get; set; }
        public string WhitePlayerId { get; set; }
        public string BlackPlayerId { get; set; }
        public TimeControlDto? TimeControl { get; set; }
        public bool Rating { get; set; }
    }
}
