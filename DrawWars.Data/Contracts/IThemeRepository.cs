using DrawWars.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DrawWars.Data.Contracts
{
    public interface IThemeRepository
    {
        Task<Theme> GetAsync(int id);

        Task<IEnumerable<Theme>> ListRandomAsync(int count);
    }
}
