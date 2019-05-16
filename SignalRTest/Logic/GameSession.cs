using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRTest.Logic
{
    public class GameSession
    {
        private const int MaxLotation = 6;
        public string Room { get; set; }
        public Guid SessionId { get; set; }
        public List<Player> players;
        public Phases GamePhase { get; set; }
        public string UiClientConnection  { get; set; }

        public Dictionary<Guid, List<string>> GetThemes()
        {
            var themes = new Dictionary<Guid, List<string>>();
            int counter = 1;
            players.ForEach(p => themes.Add(p.PlayerId, new List<string> { $"Tema {counter++}" }));
            return themes;
        }

        internal void setArt(Guid playerId, byte[] draw)
        {
            players.Where(p => p.PlayerId == playerId).FirstOrDefault()?.Draws.Add(new Art(draw));
        }
    }
}
