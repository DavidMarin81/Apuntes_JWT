using Prueba02JWT.Dtos;

namespace Prueba02JWT.Services
{
    public interface IAuthService
    {
        Task<UserDto?> RegisterAsync(RegisterDto dto);
        Task<string?> LoginAsync(LoginDto dto);
    }
}
