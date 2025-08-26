using Shared.Models;

namespace DataLayer
{
    public class Game
    {
        public string WhitePlayerId { get; set; }
        public string BlackPlayerId { get; set; }
        public bool Rating { get; set; }
        public PlayerColor Turn {  get; set; }
        public int MoveCount {  get; set; }
    }
}
