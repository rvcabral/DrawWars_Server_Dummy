namespace DrawWars.Entities
{
    public sealed class Drawing : BaseEntity
    {
        public int GameRoomId { get; set; }

        public int PlayerId { get; set; }

        public int ThemeId { get; set; }

        public string Url { get; set; }
    }
}
