using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRTest.GameManager;
using SignalRTest.Logic;

namespace SignalRTest.Hubs
{
    public class DrawWarsHub : Hub
    {
        
        #region Pre game phase

        public async Task Inlist(string room)
        {
            //Console.WriteLine($"Registering unto room {room}");
            //if(CoreManager.IsPlayerAlreadyRegistered(room, Context.ConnectionId))
            //{
            //    return;
            //}
            var res = CoreManager.inlist(room, Context.ConnectionId);
            if (res.Session.Equals(Guid.Empty))
            {
                await Clients.Caller.SendAsync("NonExistingSession", res.PlayerId);
            }
            else
            {
                await Clients.Caller.SendAsync("AckSession", res);
            }
            
        }

        public async Task RegisterUIClient()
        {
            //Note, for now we don't support a page refresh on UI so we don't need to send sessionId
            GameSession session = CoreManager.RegisterUIClient(Context.ConnectionId);
            await Clients.Caller.SendAsync("AckUIClient", new { SessionRoom = session.Room, SessionId = session.SessionId });
        }

        public async Task SetPlayerNickName(Context context, string nickname)
        {
            Console.WriteLine($"Player {context.PlayerId} from Session {context.Session} has set his nickname to {nickname}");
            var success = CoreManager.SetUserNickName(context, nickname);
            await Clients.Caller.SendAsync("AckNickname");
            string uiConId = CoreManager.GetUiClient(context.Session);
            await Clients.Client(uiConId).SendAsync("NewPlayer", nickname);
        }

        public async Task Ready(Context context)
        {
            CoreManager.StartSession(context.Session);
            var playerConnections = CoreManager.GetContextPlayerConnectionId(context);

            await Clients.Clients(playerConnections).SendAsync("DrawThemes", CoreManager.GetSession(context.Session).GetThemes());
            await Clients.Client(CoreManager.GetSession(context.Session).UiClientConnection).SendAsync("DrawThemes", 60);
        }

        #endregion

        #region Game Logic

        public async Task SetTimesUp(Guid session)
        {
            var s = CoreManager.GetSession(session);
            var uiC = s.UiClientConnection;
            
            await Clients.Clients(CoreManager.GetContextPlayerConnectionId(session)).SendAsync("TimesUp");
        }

        public async Task DrawSubmitted(Context context)
        {

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
            }
        }

        public async Task NextGamePhase(Guid session)
        {
            
            await Clients.Clients(CoreManager.GetContextPlayerConnectionId(session)).SendAsync("SeeResults");
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
                    await Clients.Client(CoreManager.GetSession(session).UiClientConnection).SendAsync("NextRound", 5);
                }
                else
                {
                    await Clients.All.SendAsync("EndOfGame");
                }
            }
        }


        public async Task RoundEndedAck(Guid session)
        {
            CoreManager.ResetRounDone(session);
            await Clients.Clients(CoreManager.GetContextPlayerConnectionId(session)).SendAsync("DrawThemes", CoreManager.GetSession(session).GetThemes());
            await Clients.Client(CoreManager.GetSession(session).UiClientConnection).SendAsync("DrawThemes", 60);
        }

        public async Task SendGuess(Context context, string guess)
        {

            var currGuess = CoreManager.GetSession(context.Session).currentTheme;
            if (guess.ToLower().Trim() == currGuess.ToLower().Trim())
            {
                CoreManager.GetSession(context.Session).PlayerGuessedCorrectly(context.PlayerId);
                await Clients.Client(CoreManager.GetSession(context.Session).UiClientConnection).SendAsync("PlayerGuess", new
                {
                    guess,
                    isCorrect = true,
                    player = CoreManager.GetSession(context.Session).GetPlayerSafe(context.PlayerId).nickname
                });

                await Clients.Caller.SendAsync("RightGuess");
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
            }

        }
        
        #endregion


    }
}
