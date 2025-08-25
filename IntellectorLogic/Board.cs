using IntellectorLogic.Pieces;
using Shared.Models;
using System.Runtime.Serialization;

namespace IntellectorLogic
{
    public class Board
    {
        public IPiece[][] Pieces { get; set; }

        public IEnumerable<Move> GetAllAvailableMoves()
        {
            return Pieces.SelectMany(row =>
                row.SelectMany(p =>
                    p?.GetAvailableMoves() ?? []
            ));
        }

        public bool CheckIfMoveIsAvailable(Move move)
        {
            var movingPiece = Pieces[move.StartX][move.StartY];
            if (movingPiece == null) return false;

            IEnumerable<Move> possibleMoves = movingPiece.GetAvailableMoves();
            return possibleMoves.Contains(move);
        }

        public bool CheckIfMoveAreWinning(Move move)
        {
            IPiece movingPiece = Pieces[move.StartX][move.StartY];
            IPiece target = Pieces[move.EndX][move.EndY];
            return CaptureIntellector() || IntellectorReachLustRank();

            bool CaptureIntellector()
            {
                return (target != null) &&
                (target.Type == PieceType.Intellector) &&
                (target.Team != movingPiece.Team);
            }

            bool IntellectorReachLustRank()
            {
                return (movingPiece.Type == PieceType.Intellector) &&
                (
                    ((movingPiece.Team == false) && (move.EndY == 6)) ||
                    ((movingPiece.Team == true) && (move.EndY == 0) && (move.EndY % 2 == 0))
                );
            }
        }

        public void ExecuteMove(Move move)
        {
            if (move.Transformation == null)
            {
                Pieces[move.EndX][move.EndY] = Pieces[move.StartX][move.StartY];
                Pieces[move.EndX][move.EndY].X = move.EndX;
                Pieces[move.EndX][move.EndY].Y = move.EndY;
                Pieces[move.EndX][move.EndY].Y = move.EndY;
            }
            else
            {
                IPiece movingPiece = Pieces[move.StartX][move.StartY];
                Pieces[move.EndX][move.EndY] = CreatePiece(move.Transformation.Value, move.EndX, move.EndY, movingPiece.Team);
            }

            Pieces[move.StartX][move.StartY] = null;
        }

        public static Board CreateWithStartPosition()
        {
            Board board = new Board();
            board.Pieces = new IPiece[9][];
            for (int i = 0; i < 9; i++)
                board.Pieces[i] = new IPiece[7 - (i % 2)];

            board.Pieces[0][0] = new Dominator(0, 0, false, board.Pieces);
            board.Pieces[1][0] = new Liberator(1, 0, false, board.Pieces);
            board.Pieces[2][0] = new Agressor(2, 0, false, board.Pieces);
            board.Pieces[3][0] = new Defensor(3, 0, false, board.Pieces);
            board.Pieces[4][0] = new Intellector(4, 0, false, board.Pieces);
            board.Pieces[5][0] = new Defensor(5, 0, false, board.Pieces);
            board.Pieces[6][0] = new Agressor(6, 0, false, board.Pieces);
            board.Pieces[7][0] = new Liberator(7, 0, false, board.Pieces);
            board.Pieces[8][0] = new Dominator(8, 0, false, board.Pieces);
            for (int i = 0; i < 9; i += 2)
                board.Pieces[i][1] = new Progressor(i, 1, false, board.Pieces);

            board.Pieces[0][6] = new Dominator(0, 6, true, board.Pieces);
            board.Pieces[1][5] = new Liberator(1, 5, true, board.Pieces);
            board.Pieces[2][6] = new Agressor(2, 6, true, board.Pieces);
            board.Pieces[3][5] = new Defensor(3, 5, true, board.Pieces);
            board.Pieces[4][6] = new Intellector(4, 6, true, board.Pieces);
            board.Pieces[5][5] = new Defensor(5, 5, true, board.Pieces);
            board.Pieces[6][6] = new Agressor(6, 6, true, board.Pieces);
            board.Pieces[7][5] = new Liberator(7, 5, true, board.Pieces);
            board.Pieces[8][6] = new Dominator(8, 6, true, board.Pieces);
            for (int i = 0; i < 9; i += 2)
                board.Pieces[i][5] = new Progressor(i, 5, true, board.Pieces);

            return board;
        }

        private IPiece CreatePiece(PieceType type, int x, int y, bool team)
        {
            return type switch
            {
                PieceType.Dominator => new Dominator(x, y, team, Pieces),
                PieceType.Liberator => new Liberator(x, y, team, Pieces),
                PieceType.Agressor => new Agressor(x, y, team, Pieces),
                PieceType.Progressor => new Progressor(x, y, team, Pieces),
                PieceType.Defensor => new Defensor(x, y, team, Pieces),
                PieceType.Intellector => new Intellector(x, y, team, Pieces)
            };
        }

        [OnDeserialized]
        internal void OnDeserialized(StreamingContext context)
        {
            foreach (var row in Pieces)
            {
                foreach (var piece in row)
                {
                    if (piece != null)
                        piece.Board = Pieces;
                }
            }
        }
    }
}
