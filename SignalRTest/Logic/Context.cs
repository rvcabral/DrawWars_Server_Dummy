using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRTest.Logic
{
    public class Context
    {
        public Guid Session { get; set; }
        public Guid PlayerId { get; set; }
        
        public Context(Guid ss, Guid pid)
        {
            Session = ss;
            PlayerId = pid;
        }
    }
}
