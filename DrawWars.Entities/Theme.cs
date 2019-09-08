using Dapper.Contrib.Extensions;

namespace DrawWars.Entities
{
    [Table("Theme")]
    public sealed class Theme : BaseEntity
    {
        public string Text { get; set; }
    }
}
