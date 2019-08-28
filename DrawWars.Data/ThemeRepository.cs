using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;

namespace DrawWars.Data
{
    public class ThemeRepository : BaseRepository<Theme>, IThemeRepository
    {
        public ThemeRepository(IConfiguration config) : base(config) { }
        
        public IEnumerable<Theme> ListRandom(int count)
        {
            return ExecuteOnConnection(connection =>
            {
                return connection.Query<Theme>(
                    sql: $@"SELECT TOP {count} * 
                            FROM Theme
                            ORDER BY NEWID()",
                    commandType: CommandType.Text
                );
            });
        }
    }
}
