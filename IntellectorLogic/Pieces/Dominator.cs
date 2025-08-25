using Shared.Models;
using System.Collections.Generic;

namespace IntellectorLogic.Pieces
{
    public class Dominator : Piece
    {
        public Dominator(int x, int y, bool team, IPiece[][] board) : base(x, y, team, board)
        {
        }

        public override PieceType Type => PieceType.Dominator;
        public override List<Move> GetAvailableMoves()
        {
            List<Move> result = new List<Move>();

            //ходы вверх
            for (int j = Y + 1; j < Board[X].Length; j++)
            {
                if (Board[X][j] != null && Board[X][j].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(X, j));
                if (Board[X][j] != null && Board[X][j].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы вниз
            for (int j = Y - 1; j >= 0; j--)
            {
                if (Board[X][j] != null && Board[X][j].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(X, j));
                if (Board[X][j] != null && Board[X][j].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы по диагонали вверх вправо
            for (int i = X + 1, j = Y; i <= 8; i++)
            {
                if (i % 2 == 0) j++;
                if (j >= Board[i].Length) break;                                             //верхняя граница

                if (Board[i][j] != null && Board[i][j].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(i, j));
                if (Board[i][j] != null && Board[i][j].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы по диагонали вниз вправо
            for (int i = X + 1, j = Y; i <= 8; i++)
            {
                if (i % 2 == 1) j--;
                if (j < 0) break;                                                                   //нижняя граница

                if (Board[i][j] != null && Board[i][j].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(i, j));
                if (Board[i][j] != null && Board[i][j].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы по диагонали вверх влево
            for (int i = X - 1, j = Y; i >= 0; i--)
            {
                if (i % 2 == 0) j++;
                if (j >= Board[i].Length) break;                                             //верхняя граница

                if (Board[i][j] != null && Board[i][j].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(i, j));
                if (Board[i][j] != null && Board[i][j].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы по диагонали вниз влево
            for (int i = X - 1, j = Y; i >= 0; i--)
            {
                if (i % 2 == 1) j--;
                if (j < 0) break;                                                                   //нижняя граница

                if (Board[i][j] != null && Board[i][j].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(i, j));
                if (Board[i][j] != null && Board[i][j].Team != Team) break;      //есть фигура и она вражеская
            }

            if (HasIntellectorNearby())
            {
                result = AddMovesWithCaptureTransformation(result);
            }
            return result;
        }
    }
}