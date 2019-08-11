using Dapper.Contrib.Extensions;

namespace DrawWars.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
    }
}
