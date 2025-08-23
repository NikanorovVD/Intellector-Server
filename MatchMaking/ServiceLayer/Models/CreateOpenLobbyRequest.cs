﻿using Shared.Models;

namespace ServiceLayer.Models
{
    public class CreateOpenLobbyRequest
    {
        public TimeControlDto? TimeControl { get; set; }
        public ColorChoice ColorChoice { get; set; }
        public bool Rating { get; set; }
    }
}
