using Shared.Models;

namespace ServiceLayer.Models
{
    public class GameResultMessage
    {
        public GameResult Result { get; set; }
        public GameResultReason Reason { get; set; }
        public GameResultMessage()
        {}
        public GameResultMessage(GameResult result, GameResultReason reason)
        {
            Result = result;
            Reason = reason;
        }
    }
}
