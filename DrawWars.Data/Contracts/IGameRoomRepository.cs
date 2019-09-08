using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IGameRoomRepository
    {
        GameRoom Create(GameRoom gameRoom);

        List<GameRoom> List(int page, int pageSize);
    }
}
