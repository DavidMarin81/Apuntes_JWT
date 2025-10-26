using Prueba02JWT.Models;

namespace Prueba02JWT.Repository
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string name);
        Task<Role?> GetByIdAsync(int id);

    }
}
