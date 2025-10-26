using BCrypt.Net;
using Microsoft.IdentityModel.Tokens;
using Prueba02JWT.Dtos;
using Prueba02JWT.Models;
using Prueba02JWT.Repository;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Prueba02JWT.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IRoleRepository roleRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
        }

        public async Task<UserDto?> RegisterAsync(RegisterDto dto)
        {
            // 1. Verificar si el email ya existe
            var existingUser = await _userRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null) return null;

            // 2. Obtener rol
            var role = await _roleRepository.GetByNameAsync(dto.Role);
            if (role == null) return null;

            // 3. Hashear contraseña
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 4. Crear usuario
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = passwordHash,
                RoleId = role.Id
            };

            user.Id = await _userRepository.CreateUserAsync(user);
            user.Role = role;

            // 5. Devolver UserDto
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role.Name
            };
        }

        public async Task<string?> LoginAsync(LoginDto dto)
        {
            // 1. Buscar usuario por email
            var user = await _userRepository.GetByEmailAsync(dto.Email);
            if (user == null) return null;

            // 2. Verificar contraseña
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

            // 3. Asignamos el role manualmente según el email
            // 3. Asignar rol según email
            string roleName;
            if (dto.Email.Contains("master"))
                roleName = "master";
            else if (dto.Email.Contains("intranet"))
                roleName = "intranet";
            else if (dto.Email.Contains("@"))
                roleName = "normal";
            else
                roleName = "normal"; // default

            user.Role = new Role { Id = 0, Name = roleName }; // Id no nos importa para este ejemplo


            // 3. Generar JWT
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role!.Name)
                }),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"])),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

