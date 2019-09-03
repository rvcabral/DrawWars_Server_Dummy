using Dapper.Contrib.Extensions;
using System;

namespace DrawWars.Entities
{
    [Table("GameRoom")]
    public sealed class GameRoom : BaseEntity
    {
        public string Code { get; set; }

        public DateTime CreationDate { get; set; }

        public Guid SessionUuid { get; set; }
    }
}
