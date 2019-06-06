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
        private List<Player> players;
        private object SessionLock = new object();
        public string UiClientConnection  { get; set; }
        public string currentTheme { get; set; }

        #region ctor

        public GameSession(string room)
        {
            Room = room;
            players = new List<Player>();
            SessionId = Guid.NewGuid();
        }

        #endregion

        public Dictionary<Guid, List<string>> GetThemes()
        {
            var themes = new Dictionary<Guid, List<string>>();
            int counter = 1;
            players.ForEach(p => themes.Add(p.PlayerId, new List<string> { $"Tema {counter++}" }));
            return themes;
        }

        internal object GetPlayerByConnectionIdSafe(string connectionId)
        {
            lock (SessionLock)
            {
                return players.FirstOrDefault(p => p.ConnectionId == connectionId);
            }
        }

        internal void setArt(Guid playerId, string draw, string theme)
        {
            Player player = GetPlayerSafe(playerId);
            if (player == null) return;

            player.Draws.Add(new Draw(draw,theme));
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
            lock(SessionLock)
            {
                return players.Any(p => p.Draws.Any(d => d.Shown == false));
            }
        }

        internal void ResetPlayerGuesses()
        {
            lock (SessionLock)
            {
                players.ForEach(p => p.GuessedCorrectly = false);
            }
        }

        internal void PlayerGuessedCorrectly(Guid playerId)
        {
            var player = GetPlayerSafe(playerId);
            if(player!=null)
                player.GuessedCorrectly = true;
        }

        internal Player GetPlayerSafe(Guid PlayerId)
        {
            lock (SessionLock)
            {
                return players.FirstOrDefault(p => p.PlayerId == PlayerId);
            }
        }

        internal Player AddPlayerSafe(string ConnectionId)
        {
            var player = new Player
            {
                Points = 0,
                ConnectionId = ConnectionId,
                Draws = new List<Draw>()
            };
            var inserted = false;
            do
            {
                player.PlayerId = Guid.NewGuid();
                lock (SessionLock)
                {
                    if (!players.Any(p => p.PlayerId == player.PlayerId))
                    {
                        players.Add(player);
                        inserted = true;
                    }
                }


            } while (!inserted);
            return player;
        }

        internal bool AllDrawsSubmitted()
        {
            lock (SessionLock)
            {
                return !players.Any(p => p.Draws.Count == 0);
            }
        }

        internal void ResetRounDone()
        {
            lock (SessionLock)
            {
                players.ForEach(p => p.RoundDone = false);
            }
        }

        internal bool AllPlayersReady()
        {
            lock(SessionLock)
            {
                return players.Count() == players.Where(p => p.RoundDone).Count();
            }
        }

        internal void SetAllRounDone()
        {
            lock (SessionLock)
            {
                players.ForEach(p => p.RoundDone = true);
            }
        }

        internal bool AllGuessedCorrectly()
        {
            lock(SessionLock)
            {
                return !players.Any(p => p.GuessedCorrectly == false);
            }
        }

        internal List<string> GetAllPlayersConnections()
        {
            
            lock(SessionLock)
            {
                return players.Select(p => p.ConnectionId).ToList();
            }
        }
    }
}
