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
        
        public async Task<IEnumerable<Theme>> ListRandomAsync(int count)
        {
            return await ExecuteOnConnection(async connection =>
            {
                return await connection.QueryAsync<Theme>(
                    sql: $@"SELECT TOP {count} * 
                            FROM Theme
                            ORDER BY NEWID()",
                    commandType: CommandType.Text
                );
            });
        }
    }
}
