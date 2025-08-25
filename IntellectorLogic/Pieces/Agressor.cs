using Shared.Models;

namespace IntellectorLogic.Pieces
{
    public class Agressor : Piece
    {
        public Agressor(int x, int y, bool team, IPiece[][] board) : base(x, y, team, board)
        { }
        public override PieceType Type => PieceType.Agressor;
        public override List<Move> GetAvailableMoves()
        {
            List<Move> result = new List<Move>();

            //ходы вправо
            for (int i = X + 2; i <= 8; i += 2)
            {
                if (Board[i][Y] != null && Board[i][Y].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(i, Y));
                if (Board[i][Y] != null && Board[i][Y].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы влево
            for (int i = X - 2; i >= 0; i -= 2)
            {
                if (Board[i][Y] != null && Board[i][Y].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(i, Y));
                if (Board[i][Y] != null && Board[i][Y].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы по диагонали вверх вправо
            for (int i = X + 1, j = Y + 1; i <= 8; i++, j++)
            {
                if (i % 2 == 0) j++;
                if (j >= Board[i].Length) break;                                             //верхняя граница

                if (Board[i][j] != null && Board[i][j].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(i, j));
                if (Board[i][j] != null && Board[i][j].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы по диагонали вниз вправо
            for (int i = X + 1, j = Y - 1; i <= 8; i++, j--)
            {
                if (i % 2 == 1) j--;
                if (j < 0) break;                                                                   //нижняя граница

                if (Board[i][j] != null && Board[i][j].Team == Team) break;      //есть фигура и она союзная
                result.Add(CreateMove(i, j));
                if (Board[i][j] != null && Board[i][j].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы по диагонали вверх влево
            for (int i = X - 1, j = Y + 1; i >= 0; i--, j++)
            {
                if (i % 2 == 0) j++;
                if (j >= Board[i].Length) break;                                             //верхняя граница

                if (Board[i][j] != null && Board[i][j].Team == Team) break;     //есть фигура и она союзная
                result.Add(CreateMove(i, j));
                if (Board[i][j] != null && Board[i][j].Team != Team) break;      //есть фигура и она вражеская
            }

            //ходы по диагонали вниз влево
            for (int i = X - 1, j = Y - 1; i >= 0; i--, j--)
            {
                if (i % 2 == 1) j--;
                if (j < 0) break;                                                                   //нижняя граница

                if (Board[i][j] != null && Board[i][j].Team == Team) break;     //есть фигура и она союзная
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