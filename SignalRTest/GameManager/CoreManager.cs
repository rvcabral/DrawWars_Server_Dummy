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

        internal static void ResetRounDone(Context context)
        {
            Sessions.Where(s => s.SessionId == context.Session)
                .FirstOrDefault()
                .players
                .ForEach(p => p.RoundDone = false);
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
    }
}
