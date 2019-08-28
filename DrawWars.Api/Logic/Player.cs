using System;
using System.Collections.Generic;

namespace DrawWars.Api.Logic
{
    public class Player
    {
        public Guid PlayerId { get; set; }
        public string ConnectionId { get; set; }
        public string DeviceId { get; set; }
        public int Points { get; set; }
        public string nickname { get; set; }
        public int InteractionCounter { get; set; }
        public Draw Draw { get; set; }
        public bool RoundDone { get; set; }
        public bool GuessedCorrectly { get; set; }
    }
}