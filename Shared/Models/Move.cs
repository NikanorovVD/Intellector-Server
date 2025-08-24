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

        public override string ToString()
        {
            string transformStr = Transformation == null ? string.Empty : $" : {Transformation}";
            return $"{StartX} {StartY} -> {EndX} {EndY}{transformStr}";
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
