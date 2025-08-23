using Shared.Models;

namespace ServiceLayer.Models
{
    public class OpenLobbyDto
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public TimeControlDto? TimeControl { get; set; }
        public ColorChoice ColorChoice { get; set; }
        public bool Rating { get; set; }
    }
}
