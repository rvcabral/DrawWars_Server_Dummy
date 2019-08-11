using Dapper;
using Dapper.Contrib.Extensions;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DrawWars.Data
{
    public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(IConfiguration config) : base(config) { }
        
        public async Task<IEnumerable<Player>> ListByGameRoomAsync(int gameRoomId)
        {
            return await ExecuteOnConnection(async connection =>
            {
                return await connection.QueryAsync<Player>(
                    sql: $"SELECT * FROM {nameof(Player)} WHERE {nameof(Player.GameRoomId)} = @{nameof(gameRoomId)}",
                    param: new { gameRoomId },
                    commandType: CommandType.Text
                );
            });
        }
    }
}
