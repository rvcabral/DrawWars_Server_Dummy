using System.Collections.Generic;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using Dapper.Contrib.Extensions;
using System.Threading.Tasks;
using Dapper;
using System.Data;

namespace DrawWars.Data
{
    public class DrawingRepository : BaseRepository<Drawing>, IDrawingRepository
    {
        public DrawingRepository(IConfiguration config) : base(config) { }
        
        public async Task<IEnumerable<Drawing>> ListByGameRoomAsync(int gameRoomId)
        {
            return await ExecuteOnConnection(async connection => 
            {
                return await connection.QueryAsync<Drawing>(
                    sql: $"SELECT * FROM {nameof(Drawing)} WHERE {nameof(Drawing.GameRoomId)} = @{nameof(gameRoomId)}",
                    param: new { gameRoomId },
                    commandType: CommandType.Text
                );
            });
        }

        public async Task<IEnumerable<Drawing>> ListByPlayerAsync(int playerId)
        {
            return await ExecuteOnConnection(async connection =>
            {
                return await connection.QueryAsync<Drawing>(
                    sql: $"SELECT * FROM {nameof(Drawing)} WHERE {nameof(Drawing.PlayerId)} = @{nameof(playerId)}",
                    param: new { playerId },
                    commandType: CommandType.Text
                );
            });
        }
    }
}
