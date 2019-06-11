using SignalRTest.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SignalRTest.GameManager
{
    public static class CoreManager
    {
        public static List<GameSession> Sessions = new List<GameSession>();
        private static object SessionsLock = new object();
        private static Random random = new Random();
        private static bool CleaningThreadRunnig = false;
        private static object threadLock = new object();

        private static Thread CleaningThread = new Thread(() =>
        {
            while (true)
            {
                Thread.Sleep(5 * 60 * 1000);
                lock (SessionsLock)
                {
                    foreach(var sess in Sessions)
                    {
                        TimeSpan ts = DateTime.Now - sess.StartMoment;
                        if (ts.TotalMinutes > 30)
                            Sessions.Remove(sess);
                    }
                }
            }

        });

        #region Concurrent Safe Util Functions

        private static void StartInitCleaingThread()
        {
            var switched = false;
            lock (threadLock)
            {
                if (!CleaningThreadRunnig) {
                    CleaningThreadRunnig = true;
                    switched = true;
                }
            }
            if (switched)
                CleaningThread.Start();
        }

        private static GameSession GetSessionByIdSafe(Guid session)
        {
            GameSession gs = null; 
            lock (SessionsLock)
            {
                gs = Sessions.FirstOrDefault(s => s.SessionId == session);
            }
            return gs;
        }

        internal static bool IsPlayerAlreadyRegistered(string room, string connectionId)
        {
            var session = GetSessionByRoomSafe(room);
            if (session == null) return true;
            return session.GetPlayerByConnectionIdSafe(connectionId)!=null;
        }

        internal static void StartSession(Guid session)
        {
            var s = GetSessionByIdSafe(session);
            if (s == null) return;
            s.StartSession();
        }

        private static GameSession GetSessionByRoomSafe(string room)
        {
            room = room.ToUpper();
            GameSession gs = null;
            lock (SessionsLock)
            {
                gs = Sessions.FirstOrDefault(s => s.Room == room);
            }
            return gs;
        }

        private static bool AddSessionSafe(GameSession session)
        {
            var ok = true;
            lock (SessionsLock)
            {
                if (!Sessions.Any(s => s.SessionId == session.SessionId || s.Room == session.Room))
                    Sessions.Add(session);

                else ok = false;
            }
            lock(threadLock)
            {
                if (!CleaningThreadRunnig)
                    StartInitCleaingThread();
            }
            return ok;

        }

        private static string GetUiClientSafe(Guid session)
        {
            lock (SessionsLock)
            {
                return Sessions.FirstOrDefault(s => s.SessionId == session)?.UiClientConnection;
            }
        }

        #endregion

        public static Context inlist(string room, string connectionId)
        {
            
            GameSession session = GetSessionByRoomSafe(room);
            if (session != null && !session.HasStarted())
            {
                var player = session.AddPlayerSafe(connectionId);
                return new Context(session.SessionId, player.PlayerId);
            }

            return new Context(Guid.Empty, Guid.Empty);
        }

        internal static void setDraw(Context context, string uri, string Theme)
        {
            var session = GetSessionByIdSafe(context.Session);
            if(session!=null)
                session.setArt(context.PlayerId,uri, Theme);
        }

        internal static bool ValidContext(Context context)
        {
            var session = GetSessionByIdSafe(context.Session);
            if (session == null) return false;
            var player = session.GetPlayerSafe(context.PlayerId);
            if (player == null) return false;
            return true;
        }

        internal static string GetUiClient(Guid session)
        {
            return GetUiClientSafe(session);
        }

        internal static bool IsEndOfSession(Guid session)
        {
            return GetSessionByIdSafe(session).IsEndOfSession();
        }

        internal static void CleanDraws(Guid session)
        {
            GetSessionByIdSafe(session).CleanDraws();
        }

        internal static bool AllReady(Context context)
        {
            var session = GetSession(context.Session);
            if (session == null) throw new Exception($"No such session find. {context.Session}");
            return session.AllPlayersReady();
        }

        internal static GameSession GetSession(Guid session)
        {
            return GetSessionByIdSafe(session);
        }
        

        internal static void SetRounDone(Context context)
        {
            var session = GetSessionByIdSafe(context.Session);
            if (session == null) return;
            var player = session.GetPlayerSafe(context.PlayerId);
            if (player == null) return;
            player.RoundDone = true;
        }

        internal static bool AllDrawsSubmitted(Context context)
        {
            var session = GetSession(context.Session);
            if (session == null) throw new Exception($"No such session find. {context.Session}");
            return session.AllDrawsSubmitted();
        }

        internal static void ResetRounDone(Guid session)
        {
            var s = GetSessionByIdSafe(session);
            if (s == null) throw new Exception($"No such session find. {session}");
            s.ResetPlayerData();
        }
        internal static void SetAllRounDone(Guid session)
        {
            var s = GetSessionByIdSafe(session);
            if (s == null) throw new Exception($"No such session find. {session}");
            s.SetAllRounDone();
        }

        internal static bool SetUserNickName(Context context, string nickname)
        {
            var sess = GetSessionByIdSafe(context.Session);
            if (sess == null) return false;
            var plr = sess.GetPlayerSafe(context.PlayerId);
            if (plr == null) return false;
            plr.nickname = nickname;
            return true;
        }

        

        internal static GameSession RegisterUIClient(string connection)
        {
            var session = new GameSession(GenerateRoomCode(), connection);
            AddSessionSafe(session);
            return session;
        }

        internal static bool AllGuessedCorrectly(Context context)
        {
            var sess = GetSessionByIdSafe(context.Session);
            if (sess == null) return false;
            return sess.AllGuessedCorrectly();
        }

        internal static List<string> GetContextConnectionIds(Context context)
        {
            var session = GetSessionByIdSafe(context.Session);
            List<string> connections = new List<string>();
            connections.Add(GetUiClientSafe(context.Session));
            connections.AddRange(session.GetAllPlayersConnections());

            return connections;
        }

        internal static List<string> GetContextPlayerConnectionId(Context context)
        {
            return GetContextPlayerConnectionId(context.Session);
        }

        internal static List<string> GetContextPlayerConnectionId(Guid Session)
        {
            var session = GetSessionByIdSafe(Session);
            if (session == null) throw new Exception($"No such session find. {Session}");
            return session.GetAllPlayersConnections();
        }
        internal static List<string> GetContextPlayerConnectionIdExcept(Guid Session, Guid excludePlayerId)
        {
            var session = GetSessionByIdSafe(Session);
            if (session == null) throw new Exception($"No such session find. {Session}");
            return session.GetPlayersConnectionExcept(excludePlayerId);
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
            while (GetSessionByRoomSafe(newCode)!=null);
            return newCode;
        }
    }
}
