using Dapper;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Linq;

namespace DrawWars.Data
{
    public class DrawWarsUserRepository : BaseRepository<DrawWarsUser>, IDrawWarsUserRepository
    {
        public DrawWarsUserRepository(IConfiguration config) : base(config) { }

        public DrawWarsUser GetByUsername(string username)
        {
            return ExecuteOnConnection(connection =>
                connection.Query<DrawWarsUser>(
                    sql: "SELECT TOP 1 * FROM DrawWarsUser WHERE Username = @username",
                    commandType: CommandType.Text,
                    param: new { username }
                ).SingleOrDefault()
            );
        }
    }
}
