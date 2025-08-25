using Shared.Models;

namespace IntellectorLogic
{
    public interface IPiece
    {
        public PieceType Type { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Team { get; set; }
        public IPiece[][] Board { get; set; }

        public bool HasIntellectorNearby();
        abstract public List<Move> GetAvailableMoves();
    }
}