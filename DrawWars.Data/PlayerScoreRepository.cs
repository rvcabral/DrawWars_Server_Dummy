using Dapper;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DrawWars.Data
{
    public class PlayerScoreRepository : BaseRepository<PlayerScore>, IPlayerScoreRepository
    {
        public PlayerScoreRepository(IConfiguration config) : base(config) { }

        public IEnumerable<PlayerScore> GetByGameRoom(int gameRoomId)
        {
            return ExecuteOnConnection(connection =>
            {
                return connection.Query<PlayerScore>(
                    sql: $"SELECT * FROM {nameof(PlayerScore)} WHERE {nameof(PlayerScore.GameRoomId)} = @{nameof(gameRoomId)}",
                    param: new { gameRoomId },
                    commandType: CommandType.Text
                );
            });
        }

        public IEnumerable<PlayerScore> GetByPlayer(int playerId)
        {
            return ExecuteOnConnection(connection =>
            {
                return connection.Query<PlayerScore>(
                    sql: $"SELECT * FROM {nameof(PlayerScore)} WHERE {nameof(PlayerScore.PlayerId)} = @{nameof(playerId)}",
                    param: new { playerId },
                    commandType: CommandType.Text
                );
            });
        }
    }
}
