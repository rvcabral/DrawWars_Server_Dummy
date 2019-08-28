using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IPlayerRepository
    {
        Player Create(Player player);

        Player Get(int id);

        IEnumerable<Player> ListByGameRoom(int gameRoomId);
    }
}
