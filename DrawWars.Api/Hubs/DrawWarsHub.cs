using DrawWars.Api.GameManager;
using DrawWars.Api.Logic;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace DrawWars.Api.Hubs
{
    public class DrawWarsHub : Hub
    {
        
        #region Pre game phase

        public async Task Inlist(string room, string deviceId)
        {
            var res = CoreManager.inlist(room.ToUpper(), Context.ConnectionId, deviceId);
            if (res.Session.Equals(Guid.Empty))
            {
                await Clients.Caller.SendAsync("NonExistingSession", res.PlayerId);
            }
            else
            {
                await Clients.Caller.SendAsync("AckSession", res);
                CoreManager.IncrementPlayerInteraction(res);
            }
            
        }

        public async Task ConnectionIdMightHaveChanged(Context context)
        {
            CoreManager.UpdateConnectionId(context, Context.ConnectionId);
            return;
        }

        public async Task RegisterUIClient()
        {
            //Note, for now we don't support a page refresh on UI so we don't need to send sessionId
            GameSession session = CoreManager.RegisterUIClient(Context.ConnectionId);
            await Clients.Caller.SendAsync("AckUIClient", new { SessionRoom = session.Room, SessionId = session.SessionId });
        }

        public async Task SetPlayerNickName(Context context, string nickname)
        {
            CoreManager.IncrementPlayerInteraction(context);
            Console.WriteLine($"Player {context.PlayerId} from Session {context.Session} has set his nickname to {nickname}");
            var success = CoreManager.SetUserNickName(context, nickname);
            await Clients.Caller.SendAsync("AckNickname");
            CoreManager.IncrementPlayerInteraction(context);
            string uiConId = CoreManager.GetUiClient(context.Session);
            await Clients.Client(uiConId).SendAsync("NewPlayer", nickname);
        }

        public async Task Ready(Context context)
        {
            CoreManager.IncrementPlayerInteraction(context);
            CoreManager.StartSession(context.Session);
            var playerConnections = CoreManager.GetContextPlayerConnectionId(context);

            await Clients.Clients(playerConnections).SendAsync("DrawThemes", CoreManager.GetSession(context.Session).GetThemes());
            CoreManager.IncrementAllPlayersInteraction(context);
            

            await Clients.Client(CoreManager.GetSession(context.Session).UiClientConnection).SendAsync("DrawThemes", 60);
        }

        #endregion

        #region Game Logic

        public async Task SetTimesUp(Guid session)
        {
            await Clients.Clients(CoreManager.GetContextPlayerConnectionId(session)).SendAsync("TimesUp");
            CoreManager.IncrementAllPlayersInteraction(session);
        }

        public async Task DrawSubmitted(Context context)
        {
            CoreManager.IncrementPlayerInteraction(context);
            if (CoreManager.AllDrawsSubmitted(context))
            {
                await Clients.Clients(CoreManager.GetUiClient(context.Session)).SendAsync("ReadyToShowDraws", 5);
            }
        }

        public async Task DrawPhaseLogic(Guid session)
        {
            var nextDraw = CoreManager.GetSession(session).nextDraw();
            if (nextDraw!=null && !string.IsNullOrWhiteSpace(nextDraw.DrawUri))
            {
                CoreManager.GetSession(session).ResetRounDone();
                var owner = CoreManager.GetSession(session).GetPlayerSafe(nextDraw.Owner);
                
                await Clients.Clients(CoreManager.GetContextPlayerConnectionIdExcept(session, nextDraw.Owner)).SendAsync("TryAndGuess");
                await Clients.Client(owner.ConnectionId).SendAsync("StandBy");

                CoreManager.IncrementAllPlayersInteraction(session);

                await Clients.Client(CoreManager.GetUiClient(session))
                .SendAsync("ShowDrawing", new
                {
                    drawUrl = nextDraw,
                    timeout = 60
                });
            }
            else
            {
                await Clients.All.SendAsync("EndOfGame");
                CoreManager.IncrementAllPlayersInteraction(session);
            }
        }

        public async Task NextGamePhase(Guid session)
        {
            
            await Clients.Clients(CoreManager.GetContextPlayerConnectionId(session)).SendAsync("SeeResults");
            CoreManager.IncrementAllPlayersInteraction(session);
            await Clients.Client(CoreManager.GetSession(session).UiClientConnection).SendAsync("SeeResults", CoreManager.GetSession(session).GetPlayersScores());

        }

        public async Task ResultsShown(Guid session)
        {
            if (!CoreManager.GetSession(session).AllDrawsShown())
                await DrawPhaseLogic(session);

            else
            {
                if(!CoreManager.IsEndOfSession(session)){
                    CoreManager.ResetRounDone(session);
                    CoreManager.CleanDraws(session);
                    await Clients.Clients(CoreManager.GetContextPlayerConnectionId(session)).SendAsync("NextRound");
                    CoreManager.IncrementAllPlayersInteraction(session);
                    await Clients.Client(CoreManager.GetSession(session).UiClientConnection).SendAsync("NextRound", 5);
                }
                else
                {
                    await Clients.All.SendAsync("EndOfGame");
                    CoreManager.IncrementAllPlayersInteraction(session);
                }
            }
        }


        public async Task RoundEndedAck(Guid session)
        {
            CoreManager.ResetRounDone(session);
            await Clients.Clients(CoreManager.GetContextPlayerConnectionId(session)).SendAsync("DrawThemes", CoreManager.GetSession(session).GetThemes());
            CoreManager.IncrementAllPlayersInteraction(session);
            await Clients.Client(CoreManager.GetSession(session).UiClientConnection).SendAsync("DrawThemes", 60);
        }

        public async Task SendGuess(Context context, string guess)
        {
            CoreManager.IncrementPlayerInteraction(context);
            var currTheme = CoreManager.GetSession(context.Session).currentTheme;
            if(Themes.IsCorrect(currTheme, guess))
            {
                CoreManager.GetSession(context.Session).PlayerGuessedCorrectly(context.PlayerId);
                await Clients.Client(CoreManager.GetSession(context.Session).UiClientConnection).SendAsync("PlayerGuess", new
                {
                    guess,
                    isCorrect = true,
                    player = CoreManager.GetSession(context.Session).GetPlayerSafe(context.PlayerId).nickname
                });

                await Clients.Caller.SendAsync("RightGuess");
                CoreManager.IncrementPlayerInteraction(context);
                if (CoreManager.AllGuessedCorrectly(context))
                    await NextGamePhase(context.Session);
            }
            else
            {
                await Clients.Client(CoreManager.GetSession(context.Session).UiClientConnection).SendAsync("PlayerGuess", new
                {
                    guess,
                    isCorrect = false,
                    player = CoreManager.GetSession(context.Session).GetPlayerSafe(context.PlayerId).nickname
                });
                await Clients.Caller.SendAsync("WrongGuess");
                CoreManager.IncrementPlayerInteraction(context);
            }

        }
        
       
        #endregion
    }
}
