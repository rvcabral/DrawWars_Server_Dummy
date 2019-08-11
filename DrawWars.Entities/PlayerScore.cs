namespace DrawWars.Entities
{
    public sealed class PlayerScore : BaseEntity
    {
        public int GameRoomId { get; set; }

        public int PlayerId { get; set; }

        public int Score { get; set; }
    }
}
