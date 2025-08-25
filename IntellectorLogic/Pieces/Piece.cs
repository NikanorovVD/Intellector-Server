using Shared.Models;

namespace IntellectorLogic.Pieces
{
    public abstract class Piece : IPiece
    {
        public abstract PieceType Type { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Team { get; set; }
        public IPiece[][] Board { get; set; }
        protected Piece(int x, int y, bool team, IPiece[][] board)
        {
            X = x;
            Y = y;
            Team = team;
            Board = board;
        }

        public bool HasIntellectorNearby()
        {
            for (int i = X - 1; i <= X + 1; i++)
            {
                if (i < 0) continue;
                if (i > 8) continue;

                for (int j = Y - 1; j <= Y + 1; j++)
                {
                    if (j < 0) continue;
                    if (j >= Board[i].Length) continue;

                    if (X == i && Y == j) continue;
                    if (X % 2 == 0 && Y + 1 == j && X != i) continue;
                    if (X % 2 == 1 && Y - 1 == j && X != i) continue;

                    if (Board[i][j] != null && Board[i][j].Team == Team)
                        if (Board[i][j].Type == PieceType.Intellector)
                            return true;
                }
            }

            return false;
        }

        protected Move CreateMove(int newX, int newY, PieceType? transformation = null)
            => new Move(X, Y, newX, newY, transformation);

        protected List<Move> AddMovesWithCaptureTransformation(IEnumerable<Move> availableMoves)
        {
            List<Move> moves = new(availableMoves);
            foreach (Move move in availableMoves)
            {
                var target = Board[move.EndX][move.EndY];
                if (target != null && target.Team != Team)
                {
                    moves.Add(new Move(move, target.Type));
                }
            }
            return moves;
        }

        abstract public List<Move> GetAvailableMoves();
    }
}