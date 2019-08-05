using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IPlayerRepository
    {
        Task<Player> CreateAsync(Player player);

        Task<Player> GetAsync(int id);

        Task<IEnumerable<Player>> ListByGameRoomAsync(int gameRoomId);
    }
}
