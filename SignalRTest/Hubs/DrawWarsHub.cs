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
            await Clients.Caller.SendAsync("AckSession", res);
        }

        public async Task SetPlayerNickName(Context context, string nickname)
        {
            Console.WriteLine($"Player {context.PlayerId} from Session {context.Session} has set his nickname to {nickname}");
            var success = CoreManager.SetUserNickName(context, nickname);
            await Clients.Caller.SendAsync("AckNickname", success);
        }

        public async Task Ready(Context context)
        {
            CoreManager.SetRounDone(context);
            if (CoreManager.AllReady(context))
            {
                CoreManager.ResetRounDone(context);

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
            }
            return;
        }

        /*public override Task OnConnectedAsync()
        {

            return base.OnConnectedAsync();
        }*/
    }
}
