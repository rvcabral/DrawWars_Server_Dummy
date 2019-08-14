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

        public async Task<IEnumerable<GameRoom>> ListByDeviceAsync(string deviceUuid)
        {
            return await ExecuteOnConnection(async connection =>
            {
                return await connection.QueryAsync<GameRoom>(
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
