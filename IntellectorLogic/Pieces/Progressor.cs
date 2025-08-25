using Shared.Models;

namespace IntellectorLogic.Pieces
{
    public class Progressor : Piece
    {
        public Progressor(int x, int y, bool team, IPiece[][] board) : base(x, y, team, board)
        {
        }

        public override PieceType Type => PieceType.Progressor;
        public override List<Move> GetAvailableMoves()
        {
            List<Move> result = new List<Move>();

            for (int i = X - 1; i <= X + 1; i++)
            {
                if (i < 0) continue;                                                                        //левая граница
                if (i > 8) continue;                                                                         //правая граница

                for (int j = Y - 1; j <= Y + 1; j++)
                {
                    if (j < 0) continue;                                                                    //нижняя граница
                    if (j >= Board[i].Length) continue;                                              //верхняя граница

                    if (X == i && Y == j) continue;                                                         //клетка с фигурой
                    if (X % 2 == 0 && Y + 1 == j && X != i) continue;                                       //две лишние клетки сверху
                    if (X % 2 == 1 && Y - 1 == j && X != i) continue;                                       //две лишние клетки снизу

                    if (Board[i][j] != null && Board[i][j].Team == Team) continue;            //есть фигура и она союзная


                    //перемещение чёрных
                    if (!Team)
                    {
                        if (X % 2 == 0 && Y - 1 == j) continue;
                        if (X % 2 == 1 && Y + 1 != j) continue;
                    }
                    //перемещение белых
                    if (Team)
                    {
                        if (X % 2 == 0 && Y - 1 != j) continue;
                        if (X % 2 == 1 && Y + 1 == j) continue;
                    }

                    Move move = CreateMove(i, j);
                    result.Add(move);

                    // превращение при достижении последнего ряда
                    if (ReachLustRank(move))
                    {
                        result.AddRange(GetMovesWithPromotionTransformations(move));
                    }
                }
            }

            return result;
        }

        private bool ReachLustRank(Move move)
        {
            return ((Team == false) && (move.EndY == 6)) ||
                   ((Team == true) && (move.EndY == 0) && (move.EndY % 2 == 0));
        }

        private IEnumerable<Move> GetMovesWithPromotionTransformations(Move move)
        {
            return [
                new (move, PieceType.Defensor),
                new (move, PieceType.Dominator),
                new (move, PieceType.Agressor),
                new (move, PieceType.Liberator)
                ];
        }
    }
}