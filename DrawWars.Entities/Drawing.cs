using Dapper.Contrib.Extensions;

namespace DrawWars.Entities
{
    [Table("Drawing")]
    public sealed class Drawing : BaseEntity
    {
        public int GameRoomId { get; set; }

        public int PlayerId { get; set; }

        public int ThemeId { get; set; }

        public string Url { get; set; }
    }
}
