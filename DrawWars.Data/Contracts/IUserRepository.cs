using DrawWars.Entities;

namespace DrawWars.Data.Contracts
{
    public interface IDrawWarsUserRepository
    {
        DrawWarsUser GetByUsername(string username);

        DrawWarsUser Create(DrawWarsUser user);
    }
}
