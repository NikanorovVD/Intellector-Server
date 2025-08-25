using IntellectorLogic.Pieces;
using IntellectorLogic.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Shared.Models;

namespace IntellectorLogic
{

    [JsonConverter(typeof(IPieceConverter))]
    public interface IPiece
    {
        public PieceType Type { get; }
        public int X { get; set; }
        public int Y { get; set; }
        public bool Team { get; set; }

        [JsonIgnore]
        public IPiece[][]? Board { get; set; }

        public bool HasIntellectorNearby();
        abstract public List<Move> GetAvailableMoves();
    }
}