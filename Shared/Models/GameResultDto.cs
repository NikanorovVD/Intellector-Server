namespace Shared.Models
{
    public class GameResultDto
    {
        public GameResult Result { get; set; }
        public GameResultReason Reason { get; set; }
        public GameResultDto()
        { }
        public GameResultDto(GameResult result, GameResultReason reason)
        {
            Result = result;
            Reason = reason;
        }
    }
}
