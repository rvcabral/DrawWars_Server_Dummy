using SignalRTest.GameManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
        public DateTime StartMoment { get; set; }
        private bool Started { get; set; }
        private int MaxRoundScore = 0;
        private int Rounds { get; set; }
        #region ctor

        public GameSession(string room, string uiConnectionId)
        {
            Room = room;
            players = new List<Player>();
            SessionId = Guid.NewGuid();
            UiClientConnection = uiConnectionId;
            StartMoment = DateTime.Now;
            Rounds = 3;
        }

        #endregion

        public void StartSession()
        {
            lock (SessionLock)
            {
                Started = true;
                MaxRoundScore = players.Count;
            }
        }
        public bool HasStarted()
        {
            lock (SessionLock)
            {
                return Started;
            }
        }
    
        public Dictionary<Guid, List<string>> GetThemes()
        {
            var themes = new Dictionary<Guid, List<string>>();
            var random = new Random();
            players.ForEach(p => themes.Add(p.PlayerId, new List<string> { Themes.ThemesList.ElementAt(random.Next(Themes.ThemesList.Count)) }));

            
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

            player.Draw = new Draw(draw,theme, playerId);
        }

        public Draw nextDraw()
        {
            var d = players.Where(p => p.Draw.Shown == false).FirstOrDefault()?.Draw;
            if (d == null) return null;
            currentTheme = d.Theme;
            d.Shown = true;
            return d;
        }

        public bool AllDrawsShown()
        {
            lock(SessionLock)
            {
                return !players.Any(p => p.Draw.Shown == false);
            }
        }
        internal void CleanDraws()
        {
            lock (SessionLock)
            {
                players.ForEach(p =>
                {
                    p.Draw = null;
                });
            }
        }
        internal void ResetPlayerData()
        {
            lock (SessionLock)
            {
                players.ForEach(p =>
                {
                    p.GuessedCorrectly = false;
                });
            }
        }

        internal void PlayerGuessedCorrectly(Guid playerId)
        {
            var player = GetPlayerSafe(playerId);
            player.Points += Interlocked.Decrement(ref MaxRoundScore); 
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
                Draw = null
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

        internal List<PlayerResult> GetPlayersScores()
        {
            var list = new List<PlayerResult>();
            players.ForEach(p => list.Add(new PlayerResult(p.nickname, p.Points)));
            return list;
        }

        internal bool AllDrawsSubmitted()
        {
            lock (SessionLock)
            {
                return !players.Any(p => p.Draw == null);
            }
        }

        internal bool IsEndOfSession()
        {
            Rounds = Rounds - 1;
            return Rounds == 0;
        }

        internal void ResetRounDone()
        {
            lock (SessionLock)
            {
                players.ForEach(p => {
                    p.RoundDone = false;
                    p.GuessedCorrectly = false;
                });
                MaxRoundScore = players.Count;
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
                MaxRoundScore = players.Count;
            }
            
        }

        internal bool AllGuessedCorrectly()
        {
            lock(SessionLock)
            {
                return players.Count(p => p.GuessedCorrectly == false)==1;
            }
        }

        internal List<string> GetAllPlayersConnections()
        {
            
            lock(SessionLock)
            {
                return players.Select(p => p.ConnectionId).ToList();
            }
        }
        internal List<string> GetPlayersConnectionExcept(Guid pid)
        {
            lock (SessionLock)
            {
                return players.Where(p => p.PlayerId != pid).Select(p=>p.ConnectionId).ToList();
            }
        }
    }
}
