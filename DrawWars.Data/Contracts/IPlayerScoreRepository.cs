using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IPlayerScoreRepository
    {
        Task<IEnumerable<PlayerScore>> GetByGameRoomAsync(int gameRoomId);

        Task<IEnumerable<PlayerScore>> GetByPlayerAsync(int playerId);
    }
}
