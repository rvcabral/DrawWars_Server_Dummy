using Microsoft.AspNetCore.SignalR;
using SignalRTest.Hubs;
using System;

namespace SignalRTest.Logic
{
    public class Context
    {
        public Guid Session { get; set; }
        public Guid PlayerId { get; set; }

        public IHubContext<DrawWarsHub> ConnectionContext { get; set; }


        public Context(Guid ss, Guid pid)
        {
            Session = ss;
            PlayerId = pid;
        }
    }
}
