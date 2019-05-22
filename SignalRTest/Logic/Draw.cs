using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRTest.Logic
{
    public class Draw
    {
        public string DrawUri { get; set; }
        public bool Shown { get; set; }
        public string Theme { get; set; }

        public Draw(string uri, string theme)
        {
            DrawUri = uri;
            Theme = theme;
            Shown = false;
        }
    }
}
