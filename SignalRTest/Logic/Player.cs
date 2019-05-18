﻿using System;
using System.Collections.Generic;

namespace SignalRTest.Logic
{
    public class Player
    {
        public Guid PlayerId { get; set; }
        public string ConnectionId { get; set; }
        public int Points { get; set; }
        public string nickname { get; set; }
        public List<string> Draws { get; set; }
        public bool RoundDone { get; set; }
    }
}