namespace Shared.Models
{
    public class CreateGameRequest
    {
        public string WhitePlayerId { get; set; }
        public string BlackPlayerId { get; set; }
        public TimeControlDto? TimeControl { get; set; }
        public bool Rating {  get; set; }
    }
}
