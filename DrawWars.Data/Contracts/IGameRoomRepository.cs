using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IGameRoomRepository
    {
        GameRoom Create(GameRoom gameRoom);

        IEnumerable<GameRoom> ListByDevice(string device);
    }
}
