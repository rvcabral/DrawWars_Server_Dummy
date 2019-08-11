using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IDrawingRepository
    {
        Task<Drawing> CreateAsync(Drawing drawing);

        Task<IEnumerable<Drawing>> ListByGameRoomAsync(int gameRoomId);
        
        Task<IEnumerable<Drawing>> ListByPlayerAsync(int playerId);
    }
}
