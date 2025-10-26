using Prueba02JWT.Models;

namespace Prueba02JWT.Repository
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task<int> CreateUserAsync(User user);
    }
}
