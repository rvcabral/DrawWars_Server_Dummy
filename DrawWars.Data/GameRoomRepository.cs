using Dapper;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace DrawWars.Data
{
    public class GameRoomRepository : BaseRepository<GameRoom>, IGameRoomRepository
    {
        public GameRoomRepository(IConfiguration config) : base(config) { }

        public IEnumerable<GameRoom> ListByDevice(string deviceUuid)
        {
            return ExecuteOnConnection(connection =>
            {
                return connection.Query<GameRoom>(
                    sql: $@"SELECT * 
                            FROM GameRoom
                                INNER JOIN Player ON Player.GameRoomId = GameRoom.Id
                            WHERE Player.DeviceUuid = @deviceUuid",
                    param: new { deviceUuid },
                    commandType: CommandType.Text
                );
            });
        }
    }
}
