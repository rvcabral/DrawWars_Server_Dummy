using DrawWars.Api.Hubs;
using Microsoft.AspNetCore.SignalR;
using System;

namespace DrawWars.Api.Logic
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
