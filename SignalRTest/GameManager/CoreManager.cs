using SignalRTest.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRTest.GameManager
{
    public static class CoreManager
    {
        public static List<GameSession> Sessions = new List<GameSession>();
        private static Random random = new Random();

        public static Context inlist(string room, string connectionId)
        {
            GameSession session;
            var player = new Player
            {
                PlayerId = Guid.NewGuid(),
                Points = 0,
                ConnectionId = connectionId
            };
            if (Sessions.Any(s => s.Room == room))
            {
                session = Sessions.Where(s => s.Room == room).FirstOrDefault();
                session.players.Add(player);
            }
            
            else
            {
                session = new GameSession
                {
                    Room = room,
                    SessionId = Guid.NewGuid(),
                    players = new List<Player>()
                };
                session.players.Add(player);
                Sessions.Add(session);

            }
            return new Context(session.SessionId, player.PlayerId);
        }

        internal static string GetUiClient(Context context)
        {
            return Sessions.Where(s => s.SessionId == context.Session).FirstOrDefault()?.UiClientConnection;
        }

        internal static bool AllReady(Context context)
        {
            var session = Sessions.Where(s => s.SessionId == context.Session).FirstOrDefault();
            return session.players.Count() == session.players.Where(p => p.RoundDone).Count();
        }

        internal static GameSession GetSession(Context context)
        {
            return Sessions.Where(s => s.SessionId == context.Session).FirstOrDefault();
        }

        internal static void SetRounDone(Context context)
        {
            Sessions.Where(s => s.SessionId == context.Session)
                .FirstOrDefault()
                .players
                .Where(p => p.PlayerId == context.PlayerId)
                .FirstOrDefault()
                .RoundDone = true;
        }

        internal static void ResetRounDone(Guid session)
        {
            Sessions.Where(s => s.SessionId == session)
                .FirstOrDefault()
                .players
                .ForEach(p => p.RoundDone = false);
        }
        internal static void SetAllRounDone(Guid session)
        {
            Sessions.Where(s => s.SessionId == session)
                .FirstOrDefault()
                .players
                .ForEach(p => p.RoundDone = true);
            
        }

        internal static bool SetUserNickName(Context context, string nickname)
        {
            var sess = Sessions.Where(s => s.SessionId == context.Session).FirstOrDefault();
            if (sess == null) return false;
            var plr = sess.players.Where(p => p.PlayerId == context.PlayerId).FirstOrDefault();
            if (plr == null) return false;
            plr.nickname = nickname;
            return true;

        }

        internal static object NextPhase(Guid session)
        {
            var sess = Sessions.Where(s => s.SessionId == session).FirstOrDefault();
            SetAllRounDone(session);
            return sess.GamePhase;
        }

        internal static Guid RegisterUIClient(string connection)
        {
            var session = new GameSession
            {
                SessionId = Guid.NewGuid(),
                GamePhase = Phases.Introduction,
                Room = GenerateRoomCode(),
                UiClientConnection = connection
            };
            Sessions.Add(session);
            return session.SessionId;
        }
        
        private static string GenerateRoomCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string newCode = "";
            do
            {
                newCode = new string(Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)]).ToArray());
            }
            while (Sessions.Any(s => s.Room == newCode));
            return newCode;
        }
    }
}
