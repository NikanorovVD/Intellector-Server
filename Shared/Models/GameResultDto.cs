namespace Shared.Models
{
    public class GameResultDto
    {
        public string GameId {  get; set; }
        public GameResult Result { get; set; }
        public GameResultReason Reason { get; set; }
        public GameResultDto()
        { }

        public GameResultDto(string gameId, GameResult result, GameResultReason reason)
        {
            GameId = gameId;
            Result = result;
            Reason = reason;
        }
    }
}
