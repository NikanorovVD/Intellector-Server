using Shared.Models;
using System.Collections.Generic;

namespace IntellectorLogic.Pieces
{
    public class Liberator : Piece
    {
        public Liberator(int x, int y, bool team, IPiece[][] board) : base(x, y, team, board)
        {
        }

        public override PieceType Type => PieceType.Liberator;
        public override List<Move> GetAvailableMoves()
        {
            List<Move> result = new List<Move>();

            //ближний круг
            for (int i = X - 1; i <= X + 1; i++)
            {
                if (i < 0) continue;                                                                 //левая граница
                if (i > 8) continue;                                                                 //правая граница

                for (int j = Y - 1; j <= Y + 1; j++)
                {
                    if (j < 0) continue;                                                             //нижняя граница
                    if (j >= Board[i].Length) continue;                                              //верхняя граница

                    if (X == i && Y == j) continue;                                                  //клетка с фигурой
                    if (X % 2 == 0 && Y + 1 == j && X != i) continue;                                //две лишние клетки сверху
                    if (X % 2 == 1 && Y - 1 == j && X != i) continue;                                //две лишние клетки снизу

                    if (Board[i][j] != null) continue;                                               //есть фигура

                    result.Add(CreateMove(i, j));
                }
            }

            //дальний круг
            if (Y + 2 < Board[X].Length)                                                             //вверх
                if (Board[X][Y + 2] == null || Board[X][Y + 2].Team != Team)
                    result.Add(CreateMove(X, Y + 2));
            if (Y - 2 >= 0)                                                                          //вниз
                if (Board[X][Y - 2] == null || Board[X][Y - 2].Team != Team)
                    result.Add(CreateMove(X, Y - 2));

            if (Y + 1 < Board[X].Length && X + 2 <= 8)                                               //вверх вправо
                if (Board[X + 2][Y + 1] == null || Board[X + 2][Y + 1].Team != Team)
                    result.Add(CreateMove(X + 2, Y + 1));
            if (Y - 1 >= 0 && X + 2 <= 8)                                                            //вниз вправо
                if (Board[X + 2][Y - 1] == null || Board[X + 2][Y - 1].Team != Team)
                    result.Add(CreateMove(X + 2, Y - 1));

            if (Y + 1 < Board[X].Length && X - 2 >= 0)                                               //вверх влево
                if (Board[X - 2][Y + 1] == null || Board[X - 2][Y + 1].Team != Team)
                    result.Add(CreateMove(X - 2, Y + 1));
            if (Y - 1 >= 0 && X - 2 >= 0)                                                            //вниз влево
                if (Board[X - 2][Y - 1] == null || Board[X - 2][Y - 1].Team != Team)
                    result.Add(CreateMove(X - 2, Y - 1));

            if (HasIntellectorNearby())
            {
                result = AddMovesWithCaptureTransformation(result);
            }
            return result;
        }
    }
}