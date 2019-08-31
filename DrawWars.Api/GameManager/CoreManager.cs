using DrawWars.Api.Logic;
using DrawWars.Data;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Threading;

namespace DrawWars.Api.GameManager
{
    public static class CoreManager
    {
        #region Private Fields

        private static readonly ConcurrentDictionary<string, string> RoomCodeMap;

        private static readonly MemoryCache SessionCache;
        private static readonly Random Random;

        internal static IConfiguration Configuration {  get; set; }

        #endregion

        #region Static Constructor

        static CoreManager()
        {
            Random = new Random();
            RoomCodeMap = new ConcurrentDictionary<string, string>();
            SessionCache = new MemoryCache("session-cache");
        }

        #endregion

        #region Callbacks

        private static void SessionEndedCallback(CacheEntryRemovedArguments args)
        {
            var session = args.CacheItem.Value as GameSession;
            RoomCodeMap.Remove(session.Room, out string value);
        }

        internal static void UpdateConnectionId(Context context, string connectionId)
        {
            var plr = GetSessionById(context.Session)?.GetPlayerSafe(context.PlayerId);
            if(plr!=null) plr.ConnectionId = connectionId;
        }

        #endregion

        #region Concurrent Safe Util Functions

        internal static long GetSessionCount()
        {
            return RoomCodeMap.Count;
        }

        private static GameSession GetSessionById(Guid sessionId)
        {
            return SessionCache[sessionId.ToString().ToUpper()] as GameSession;
        }

        internal static bool IsPlayerAlreadyRegistered(string room, string connectionId)
        {
            var session = GetSessionByRoomCode(room);
            if (session == null)
                return true;
            return session.GetPlayerByConnectionIdSafe(connectionId) != null;
        }

        internal static void StartSession(Guid sessionId)
        {
            var session = GetSessionById(sessionId);

            if (session != null)
                session.StartSession();
        }

        private static GameSession GetSessionByRoomCode(string roomCode)
        {
            if(RoomCodeMap.TryGetValue(roomCode.ToUpper(), out string sessionId))
            {
                return SessionCache[sessionId] as GameSession;
            }

            return null;
        }

        internal static void IncrementAllPlayersInteraction(Context context)
        {
            IncrementAllPlayersInteraction(context.Session);
        }

        internal static void IncrementAllPlayersInteraction(Guid session)
        {
            var s = GetSessionById(session);
            if (s != null)
                foreach (var p in s.GetAllPlayers())
                    Interlocked.Increment(ref p.InteractionCounter);
        }

        private static bool AddSession(GameSession session)
        {
            var cacheEntryOptions = new CacheItemPolicy()
            {
                SlidingExpiration = TimeSpan.FromMinutes(10),
                Priority = CacheItemPriority.NotRemovable,
            };

            cacheEntryOptions.RemovedCallback += SessionEndedCallback;
            
            var cacheItem = new CacheItem(session.SessionId.ToString().ToUpper(), session);
            if (!SessionCache.Add(cacheItem, cacheEntryOptions))
            {
                return false;
            }

            if(!RoomCodeMap.TryAdd(session.Room.ToUpper(), session.SessionId.ToString()))
            {
                SessionCache.Remove(session.SessionId.ToString());
                return false;
            }
            
            return true;
        }

        private static string GetUiClientSafe(Guid sessionId)
        {
            return GetSessionById(sessionId)?.UiClientConnection;
        }

        #endregion

        public static Context inlist(string room, string connectionId, string deviceId)
        {
            GameSession session = GetSessionByRoomCode(room);

            if (session != null && !session.HasStarted())
            {
                var player = session.AddPlayerSafe(connectionId, deviceId);
                return new Context(session.SessionId, player.PlayerId);
            }

            return new Context(Guid.Empty, Guid.Empty);
        }

        internal static void setDraw(Context context, string uri, string Theme)
        {
            var session = GetSessionById(context.Session);
            if(session != null)
                session.setArt(context.PlayerId,uri, Theme);
        }

        internal static bool ValidContext(Context context)
        {
            var session = GetSessionById(context.Session);
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
            return GetSessionById(session).IsEndOfSession();
        }

        internal static void CleanDraws(Guid session)
        {
            GetSessionById(session).CleanDraws();
        }

        internal static bool AllReady(Context context)
        {
            var session = GetSession(context.Session);
            if (session == null) throw new Exception($"No such session find. {context.Session}");
            return session.AllPlayersReady();
        }

        internal static GameSession GetSession(Guid session)
        {
            return GetSessionById(session);
        }
        
        internal static void SetRounDone(Context context)
        {
            var session = GetSessionById(context.Session);
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
            var s = GetSessionById(session);
            if (s == null) throw new Exception($"No such session find. {session}");
            s.ResetPlayerData();
        }

        internal static void SetAllRounDone(Guid session)
        {
            var s = GetSessionById(session);
            if (s == null) throw new Exception($"No such session find. {session}");
            s.SetAllRounDone();
        }

        internal static bool SetUserNickName(Context context, string nickname)
        {
            var sess = GetSessionById(context.Session);
            if (sess == null) return false;
            var plr = sess.GetPlayerSafe(context.PlayerId);
            if (plr == null) return false;
            plr.nickname = nickname;

            var newPlayer = new Entities.Player()
            {
                PlayerUuid = plr.PlayerId,
                GameRoomId = sess.RoomID,
                Name = nickname,
                DeviceUuid = Guid.Parse(plr.DeviceId)
            };

            new PlayerRepository(Configuration).Create(newPlayer);

            plr.Id = newPlayer.Id;

            return true;
        }
        
        internal static GameSession RegisterUIClient(string connection)
        {
            var session = new GameSession(GenerateRoomCode(), connection, Configuration);

            var gameRoom = new GameRoom()
            {
                Code = session.Room,
                SessionUuid = session.SessionId,
                CreationDate = DateTime.Now
            };

            new GameRoomRepository(Configuration).Create(gameRoom);
            
            session.RoomID = gameRoom.Id;

            AddSession(session);

            return session;
        }

        internal static void IncrementPlayerInteraction(Context context)
        {
            var sess = GetSessionById(context.Session);
            if (sess == null) return;
            var plr = sess.GetPlayerSafe(context.PlayerId);
            if (plr == null) return;
            Interlocked.Increment(ref plr.InteractionCounter);
        }

        internal static bool AllGuessedCorrectly(Context context)
        {
            var sess = GetSessionById(context.Session);
            if (sess == null) return false;
            return sess.AllGuessedCorrectly();
        }

        internal static int GetPlayerInteractionCount(Context context)
        {
            return GetPlayerInteractionCount(context.Session, context.PlayerId);
        }

        internal static int GetPlayerInteractionCount(Guid session, Guid player)
        {
            var sess = GetSessionById(session);
            if (sess == null) return 0;
            var plr = sess.GetPlayerSafe(player);
            if (plr == null) return 0;
            return plr.InteractionCounter;
        }

        internal static List<string> GetContextConnectionIds(Context context)
        {
            var session = GetSessionById(context.Session);
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
            var session = GetSessionById(Session);
            if (session == null) throw new Exception($"No such session find. {Session}");
            return session.GetAllPlayersConnections();
        }

        internal static List<string> GetContextPlayerConnectionIdExcept(Guid Session, Guid excludePlayerId)
        {
            var session = GetSessionById(Session);
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
              .Select(s => s[Random.Next(s.Length)]).ToArray());
            }
            while (GetSessionByRoomCode(newCode)!=null);
            return newCode;
        }
    }
}
