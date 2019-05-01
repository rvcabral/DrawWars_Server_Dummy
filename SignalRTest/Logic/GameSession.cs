using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRTest.Logic
{
    public class GameSession
    {
        private int MaxLotation = 6;
        public string Room { get; set; }
        public Guid SessionId { get; set; }
        public List<Player> players;
        public Phases GamePhase { get; set; }
        public string UiClientConnection  { get; set; }

        public Dictionary<Guid, List<string>> GetThemes()
        {
            return new Dictionary<Guid, List<string>>();
        }
    }
}
