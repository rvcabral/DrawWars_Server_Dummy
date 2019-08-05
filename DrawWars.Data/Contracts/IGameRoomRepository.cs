using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IGameRoomRepository
    {
        Task<GameRoom> CreateAsync(GameRoom gameRoom);

        Task<IEnumerable<GameRoom>> ListByDeviceAsync(string device);
    }
}
