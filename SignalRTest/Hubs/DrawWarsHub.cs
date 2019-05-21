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
            await Clients.All.SendAsync("DrawThemes", CoreManager.GetSession(context.Session).GetThemes());
        }



        public async Task RegisterUIClient()
        {
            //Note, for now we don't support a page refresh on UI so we don't need to send sessionId
            GameSession session = CoreManager.RegisterUIClient(Context.ConnectionId);
            await Clients.Caller.SendAsync("AckUIClient", session.Room);
        }
        public async Task SetTimesUp(Guid session)
        {
            var s = CoreManager.GetSession(session);
            var uiC = s.UiClientConnection;
            await Clients.Client(uiC).SendAsync("timesAsync");
            foreach(var p in s.players)
                await Clients.Client(p.ConnectionId).SendAsync("timesUp");
        }

        public async Task DrawSubmitted(Context context)
        {
            await Clients.Client(CoreManager.GetUiClient(context))
                .SendAsync("ShowDrawing", CoreManager.GetSession(context.Session)
                .players.Where(p => p.PlayerId == context.PlayerId)
                .FirstOrDefault()
                .Draws
                .FirstOrDefault());
        }

    }
}
