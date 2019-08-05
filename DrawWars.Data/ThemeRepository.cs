using System.Collections.Generic;
using System.Threading.Tasks;
using DrawWars.Data.Contracts;
using DrawWars.Entities;
using Microsoft.Extensions.Configuration;

namespace DrawWars.Data
{
    public class ThemeRepository : BaseRepository<Theme>, IThemeRepository
    {
        public ThemeRepository(IConfiguration config) : base(config) { }
        
        public Task<IEnumerable<Theme>> ListRandomAsync(int count)
        {//TODO Implement
            throw new System.NotImplementedException();
        }
    }
}
