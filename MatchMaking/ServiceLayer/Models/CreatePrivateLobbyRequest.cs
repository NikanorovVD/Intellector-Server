using Shared.Models;

namespace ServiceLayer.Models
{
    public class CreatePrivateLobbyRequest
    {
        public string InvitedId { get; set; }
        public TimeControlDto? TimeControl { get; set; }
        public ColorChoice ColorChoice { get; set; }
        public bool Rating { get; set; }
    }
}
