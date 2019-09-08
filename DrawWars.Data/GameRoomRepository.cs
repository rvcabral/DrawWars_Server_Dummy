using Dapper;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DrawWars.Data
{
    public class GameRoomRepository : BaseRepository<GameRoom>, IGameRoomRepository
    {
        public GameRoomRepository(IConfiguration config) : base(config) { }

        public List<GameRoom> List(int page, int pageSize)
        {
            return ExecuteOnConnection(connection =>
            {
                return connection.Query<GameRoom>(
                    sql: $@"SELECT * 
                            FROM GameRoom
                            ORDER BY CreationDate DESC
                            OFFSET (@page * @pageSize) ROWS
                            FETCH NEXT @pageSize ROWS ONLY",
                    param: new { page, pageSize },
                    commandType: CommandType.Text
                ).ToList();
            });
        }
    }
}
