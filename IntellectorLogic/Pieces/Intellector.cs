using Shared.Models;

namespace IntellectorLogic.Pieces
{
    public class Intellector : Piece
    {
        public Intellector(int x, int y, bool team, IPiece[][] board) : base(x, y, team, board)
        {
        }

        public override PieceType Type => PieceType.Intellector;
        public override List<Move> GetAvailableMoves()
        {
            List<Move> result = new List<Move>();

            for (int i = X - 1; i <= X + 1; i++)
            {
                if (i < 0) continue;                                                                                //левая граница
                if (i > 8) continue;                                                                                //правая граница

                for (int j = Y - 1; j <= Y + 1; j++)
                {
                    if (j < 0) continue;                                                                            //нижняя граница
                    if (j >= Board[i].Length) continue;                                                      //верхняя граница

                    if (X == i && Y == j) continue;                                                       //клетка с фигурой
                    if (X % 2 == 0 && Y + 1 == j && X != i) continue;                                //две лишние клетки сверху
                    if (X % 2 == 1 && Y - 1 == j && X != i) continue;                                //две лишние клетки снизу

                    if (Board[i][j] != null)                                                                 //есть фигура
                        if (Board[i][j].Team != Team || Board[i][j].Type != PieceType.Defensor)  //не дефенсор своей команды (рокировка)
                            continue;

                    result.Add(CreateMove(i, j));
                }
            }

            return result;
        }
    }
}