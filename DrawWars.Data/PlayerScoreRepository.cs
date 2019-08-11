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

        public async Task<IEnumerable<PlayerScore>> GetByGameRoomAsync(int gameRoomId)
        {
            return await ExecuteOnConnection(async connection =>
            {
                return await connection.QueryAsync<PlayerScore>(
                    sql: $"SELECT * FROM {nameof(PlayerScore)} WHERE {nameof(PlayerScore.GameRoomId)} = @{nameof(gameRoomId)}",
                    param: new { gameRoomId },
                    commandType: CommandType.Text
                );
            });
        }

        public async Task<IEnumerable<PlayerScore>> GetByPlayerAsync(int playerId)
        {
            return await ExecuteOnConnection(async connection =>
            {
                return await connection.QueryAsync<PlayerScore>(
                    sql: $"SELECT * FROM {nameof(PlayerScore)} WHERE {nameof(PlayerScore.PlayerId)} = @{nameof(playerId)}",
                    param: new { playerId },
                    commandType: CommandType.Text
                );
            });
        }
    }
}
