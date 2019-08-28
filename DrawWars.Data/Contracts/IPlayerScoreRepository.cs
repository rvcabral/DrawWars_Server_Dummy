using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IPlayerScoreRepository
    {
        IEnumerable<PlayerScore> GetByGameRoom(int gameRoomId);

        IEnumerable<PlayerScore> GetByPlayer(int playerId);
    }
}
