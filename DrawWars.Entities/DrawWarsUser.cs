using Dapper.Contrib.Extensions;

namespace DrawWars.Entities
{
    [Table("DrawWarsUser")]
    public class DrawWarsUser : BaseEntity
    {
        public string Username { get; set; }

        public string PassHash { get; set; }
    }
}
