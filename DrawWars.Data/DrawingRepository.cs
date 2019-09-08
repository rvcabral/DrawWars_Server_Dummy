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
        
        public IEnumerable<Drawing> ListByGameRoom(int gameRoomId)
        {
            return ExecuteOnConnection(connection => 
            {
                return connection.Query<Drawing>(
                    sql: $"SELECT * FROM {nameof(Drawing)} WHERE {nameof(Drawing.GameRoomId)} = @{nameof(gameRoomId)}",
                    param: new { gameRoomId },
                    commandType: CommandType.Text
                );
            });
        }

        public IEnumerable<Drawing> ListByPlayer(int playerId)
        {
            return ExecuteOnConnection(connection =>
            {
                return connection.Query<Drawing>(
                    sql: $"SELECT * FROM {nameof(Drawing)} WHERE {nameof(Drawing.PlayerId)} = @{nameof(playerId)}",
                    param: new { playerId },
                    commandType: CommandType.Text
                );
            });
        }

        public IEnumerable<Drawing> ListByUser(int userId)
        {
            return ExecuteOnConnection(connection =>
            {
                return connection.Query<Drawing>(
                    sql: @" SELECT Drawing.*
                            FROM UserDevice
	                            INNER JOIN Player ON Player.DeviceUuid = UserDevice.DeviceUuid
	                            INNER JOIN Drawing ON Drawing.PlayerId = Player.Id
                            WHERE UserDevice.UserId = @userId",
                    param: new { userId },
                    commandType: CommandType.Text
                );
            });
        }
    }
}
