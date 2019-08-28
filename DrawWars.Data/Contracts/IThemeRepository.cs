using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IThemeRepository
    {
        Theme Get(int id);

        IEnumerable<Theme> ListRandom(int count);
    }
}
