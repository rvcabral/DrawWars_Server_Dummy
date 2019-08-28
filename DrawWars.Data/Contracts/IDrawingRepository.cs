using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IDrawingRepository
    {
        Drawing Create(Drawing drawing);

        IEnumerable<Drawing> ListByGameRoom(int gameRoomId);
        
        IEnumerable<Drawing> ListByPlayer(int playerId);
    }
}
