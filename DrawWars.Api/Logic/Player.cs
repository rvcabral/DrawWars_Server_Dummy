using System;

namespace DrawWars.Api.Logic
{
    public class Player
    {
        public int Id { get; set; }
        public int CurrentThemeId { get; set; }
        public Guid PlayerId { get; set; }
        public string ConnectionId { get; set; }
        public string DeviceId { get; set; }
        public int Points { get; set; }
        public string nickname { get; set; }
        public Draw Draw { get; set; }
        public bool RoundDone { get; set; }
        public bool GuessedCorrectly { get; set; }
    }
}