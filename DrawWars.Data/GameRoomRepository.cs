using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data
{
    public class GameRoomRepository : BaseRepository<GameRoom>, IGameRoomRepository
    {
        public GameRoomRepository(IConfiguration config) : base(config) { }

        public Task<IEnumerable<GameRoom>> ListByDeviceAsync(string device)
        {//TODO Implement
            throw new System.NotImplementedException();
        }
    }
}
