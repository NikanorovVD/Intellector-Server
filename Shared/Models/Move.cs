namespace Shared.Models
{
    public class Move
    {
        public int StartX { get; set; }
        public int StartY { get; set; }
        public int EndX { get; set; }
        public int EndY { get; set; }
        public PieceType? Transformation { get; set; }

        public Move()
        { }

        public Move(int startX, int startY, int endX, int endY, PieceType? transformation)
        {
            StartX = startX;
            StartY = startY;
            EndX = endX;
            EndY = endY;
            Transformation = transformation;
        }

        public Move(int startX, int startY, int endX, int endY) : this(startX, startY, endX, endY, null) { }

        public Move(Move move, PieceType transformation) : this(move.StartX, move.StartY, move.EndX, move.EndY, transformation) { }

        public override string ToString()
        {
            string transformStr = Transformation == null ? string.Empty : $" : {Transformation}";
            return $"{StartX} {StartY} -> {EndX} {EndY}{transformStr}";
        }

        public static bool operator ==(Move move1, Move move2)
        {
            if (ReferenceEquals(move1, null))
                return ReferenceEquals(move2, null);

            return move1.Equals(move2);
        }

        public static bool operator !=(Move move1, Move move2)
        {
            return !(move1 == move2);
        }

        public override bool Equals(object obj)
        {
            if (obj is Move move)
            {
                return StartX == move.StartX &&
                       StartY == move.StartY &&
                       EndX == move.EndX &&
                       EndY == move.EndY &&
                       Transformation == move.Transformation;
            }
            return false;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = 17;
                hash = hash * 23 + StartX.GetHashCode();
                hash = hash * 23 + StartY.GetHashCode();
                hash = hash * 23 + EndX.GetHashCode();
                hash = hash * 23 + EndY.GetHashCode();
                hash = hash * 23 + (Transformation?.GetHashCode() ?? 0);
                return hash;
            }
        }
    }



    public enum PieceType
    {
        Progressor = 0,
        Liberator = 1,
        Intellector = 2,
        Dominator = 3,
        Defensor = 4,
        Agressor = 5
    }
}
