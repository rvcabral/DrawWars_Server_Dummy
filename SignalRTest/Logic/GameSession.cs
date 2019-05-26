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
        public string UiClientConnection  { get; set; }
        public string currentTheme { get; set; }

        public Dictionary<Guid, List<string>> GetThemes()
        {
            var themes = new Dictionary<Guid, List<string>>();
            int counter = 1;
            players.ForEach(p => themes.Add(p.PlayerId, new List<string> { $"Tema {counter++}" }));
            return themes;
        }

        internal void setArt(Guid playerId, string draw, string theme)
        {
            players.Where(p => p.PlayerId == playerId).FirstOrDefault()?.Draws.Add(new Draw(draw,theme));
        }

        public string nextDraw()
        {
            var d = players.Where(p => p.Draws.FirstOrDefault().Shown == false).FirstOrDefault()?.Draws.FirstOrDefault();
            if (d == null) return "";
            currentTheme = d.Theme;
            d.Shown = true;
            return d.DrawUri;
        }
        public bool AllDrawsShown()
        {
            return players.Any(p => p.Draws.Any(d => d.Shown == false));
        }
    }
}
