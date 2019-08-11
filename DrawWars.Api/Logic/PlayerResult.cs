using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DrawWars.Api.Logic
{
    public class PlayerResult
    {
        public string username { get; set; }
        public int score { get; set; }
        public PlayerResult(string un, int sc)
        {
            username = un;
            score = sc;
        }
    }
}
