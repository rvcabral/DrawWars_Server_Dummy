using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using SignalRTest.GameManager;
using SignalRTest.Logic;

namespace SignalRTest.Hubs
{
    public class DrawWarsHub : Hub
    {

        public async Task Inlist(string room)
        {
            Console.WriteLine($"Registering unto room {room}");
           
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

        public async Task SetPlayerNickName(Context context, string nickname)
        {
            Console.WriteLine($"Player {context.PlayerId} from Session {context.Session} has set his nickname to {nickname}");
            var success = CoreManager.SetUserNickName(context, nickname);
            await Clients.Caller.SendAsync("AckNickname");
            string uiConId = CoreManager.GetUiClient(context);
            await Clients.Client(uiConId).SendAsync("NewPlayer", nickname);
        }

        public async Task Ready(Context context)
        {
            await Clients.All.SendAsync("DrawThemes", CoreManager.GetSession(context).GetThemes());
            /*CoreManager.SetRounDone(context);
            if (CoreManager.AllReady(context))
            {
                CoreManager.ResetRounDone(context.Session);

                switch (CoreManager.GetSession(context).GamePhase)
                {
                    case Phases.Introduction:
                        {
                            await Clients.All.SendAsync("NextStage");
                            break;
                        }
                    case Phases.Draw:
                        {
                            await Clients.All.SendAsync("DrawThemes", CoreManager.GetSession(context).GetThemes());
                            break;
                        }
                    case Phases.Guess:
                        {
                            await Clients.All.SendAsync("GuessTitle");
                            break;
                        }
                    case Phases.RoundEnd:
                        {
                            break;
                        }

                }

                return;
            }*/
            return;
        }

        public async Task SetArt(Context context, byte[] draw)
        {
            CoreManager.GetSession(context).setArt(context.PlayerId, draw);
        }

        public async Task RegisterUIClient()
        {
            //Note, for now we don't support a page refresh on UI so we don't need to send sessionId
            GameSession session = CoreManager.RegisterUIClient(Context.ConnectionId);
            await Clients.Caller.SendAsync("AckUIClient", session.Room);
        }
        public async Task SetTimesUp(Guid session)
        {
            var res = CoreManager.NextPhase(session);
            await Clients.Caller.SendAsync("setNextPhase", res);
        }
        public async Task SetNextPhase(Guid session)
        {
            var res = CoreManager.NextPhase(session);
            await Clients.Caller.SendAsync("setNextPhase", res);
        }

    }
}
