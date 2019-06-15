using System;

namespace DrawWars.Data.Drawing
{
    public class Drawing
    {
        public int ID { get; set; }

        public string Url { get; set; }

        public Guid DeviceID { get; set; }

        public Guid SessionID { get; set; }

        public DateTimeOffset DateCreated { get; set; }
    }
}
