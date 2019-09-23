using DrawWars.Api.GameManager;
using DrawWars.Data;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DrawWars.Api.Logic
{
    public class GameSession
    {
        private const int MaxLotation = 6;

        private List<Player> players;
        private object SessionLock = new object();
        
        public int RoomID { get; set; }

        public string Room { get; set; }
        public Guid SessionId { get; set; }
        public string UiClientConnection  { get; set; }
        public string currentTheme { get; set; }
        public DateTime StartMoment { get; set; }
        private bool Started { get; set; }
        private int MaxRoundScore = 0;
        private int Rounds { get; set; }

        private IConfiguration _configuration { get; }

        #region ctor

        public GameSession(string room, string uiConnectionId, IConfiguration config)
        {
            Room = room;
            players = new List<Player>();
            SessionId = Guid.NewGuid();
            UiClientConnection = uiConnectionId;
            StartMoment = DateTime.Now;
            Rounds = 3;

            _configuration = config;
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
            var themes = new ThemeRepository(_configuration).ListRandom(players.Count).ToList();
            var result = new Dictionary<Guid, List<string>>();

            var idx = 0;
            foreach(var player in players)
            {
                var theme = themes[idx++];
                player.CurrentThemeId = theme.Id;
                result.Add(player.PlayerId, new List<string>() { theme.Text });
            }
            
            return result;
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

            player.Draw = new Draw(draw, theme, playerId);            
        }

        public Draw nextDraw()
        {
            var d = players.Where(p => p.Draw.Shown == false).FirstOrDefault()?.Draw;
            if (d == null) return null;
            currentTheme = d.Theme;
            d.Shown = true;
            return d;
        }

        internal IEnumerable<Player> GetAllPlayers()
        {
            return players;
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

        internal Player AddPlayerSafe(string ConnectionId, string deviceId)
        {
            var player = new Player
            {
                Points = 0,
                ConnectionId = ConnectionId,
                DeviceId = deviceId,
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
            return players
                .Select(p => new PlayerResult(p.nickname, p.Points))
                .OrderByDescending(pr => pr.score)
                .ToList();
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

            if (Rounds != 0)
                return false;

            lock (SessionLock)
            {
                var scoreRepository = new PlayerScoreRepository(_configuration);
                players
                    .Select(p => new PlayerScore()
                    {
                        PlayerId = p.Id,
                        GameRoomId = RoomID,
                        Score = p.Points
                    })
                    .ToList()
                    .ForEach(ps => scoreRepository.Create(ps));
            }

            return true;
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
