using System;

namespace DrawWars.Entities
{
    public sealed class Player : BaseEntity
    {
        public int GameRoomId { get; set; }

        public string Name { get; set; }

        public Guid DeviceUuid { get; set; }

        public Guid PlayerUuid { get; set; }
    }
}
