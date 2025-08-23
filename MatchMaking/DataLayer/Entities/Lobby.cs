using GenericCrud;
using Shared.Models;

namespace DataLayer.Entities
{
    public class Lobby: IEntity<int>
    {
        public int Id { get; set; }
        public string OwnerId { get; set; }
        public string? InvitedId { get; set; }
        public bool AutoMatch { get; set; }
        public TimeControl? TimeControl { get; set; }
        public ColorChoice ColorChoice { get; set; }
        public bool Rating { get; set; }
        public DateTime ExpirationTime { get; set; }

        public bool Open { get => InvitedId == null && AutoMatch == false; }
    }

    public class TimeControl
    {
        public int TotalSeconds { get; set; }
        public int AddedSeconds { get; set; }
    }
}
