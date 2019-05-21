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
                ConnectionId = connectionId,
                Draws = new List<string>()
            };
            if (Sessions.Any(s => s.Room == room))
            {
                session = Sessions.Where(s => s.Room == room).FirstOrDefault();
                session.players.Add(player);
                return new Context(session.SessionId, player.PlayerId);
            }
            else //DEBUG PURPOSE ONLY
            {
                session = new GameSession
                {
                    Room = room,
                    SessionId = Guid.NewGuid(),
                    players = new List<Player>()
                };
                session.players.Add(player);
                Sessions.Add(session);
                return new Context(session.SessionId, player.PlayerId);
            }
            //return new Context(Guid.Empty, player.PlayerId);
        }

        internal static void setDraw(Context context, string uri)
        {
            Sessions.FirstOrDefault(s => s.SessionId == context.Session).setArt(context.PlayerId,uri);
        }

        internal static bool ValidContext(Context context)
        {
            return Sessions.Any(s => s.SessionId == context.Session && s.players.Any(p => p.PlayerId == context.PlayerId));
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

        internal static GameSession GetSession(Guid session)
        {
            return Sessions.Where(s => s.SessionId == session).FirstOrDefault();
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

        

        internal static GameSession RegisterUIClient(string connection)
        {
            var session = new GameSession
            {
                SessionId = Guid.NewGuid(),
                Room = GenerateRoomCode(),
                UiClientConnection = connection,
                players = new List<Player>()
            };
            Sessions.Add(session);
            return session;
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
