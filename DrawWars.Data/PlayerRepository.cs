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
        
        public IEnumerable<Player> ListByGameRoom(int gameRoomId)
        {
            return ExecuteOnConnection(connection =>
            {
                return connection.Query<Player>(
                    sql: $"SELECT * FROM {nameof(Player)} WHERE {nameof(Player.GameRoomId)} = @{nameof(gameRoomId)}",
                    param: new { gameRoomId },
                    commandType: CommandType.Text
                );
            });
        }
    }
}
