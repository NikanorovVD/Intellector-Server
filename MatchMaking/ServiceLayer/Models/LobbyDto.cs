using Shared.Models;

namespace ServiceLayer.Models
{
    public class LobbyDto
    {
        public int Id { get; set; }
        public bool Open { get; set; }
        public string OwnerId { get; set; }
        public string? InvitedId { get; set; }
        public TimeControlDto? TimeControl { get; set; }
        public ColorChoice ColorChoice { get; set; }
        public bool Rating { get; set; }
        public bool AutoMatch { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
