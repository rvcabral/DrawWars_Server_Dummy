using Dapper.Contrib.Extensions;

namespace DrawWars.Entities
{
    [Table("PlayerScore")]
    public sealed class Theme : BaseEntity
    {
        public string Text { get; set; }
    }
}
