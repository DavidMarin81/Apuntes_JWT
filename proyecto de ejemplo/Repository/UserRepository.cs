using Dapper;
using MySqlConnector;
using Prueba02JWT.Models;

namespace Prueba02JWT.Repository
{
    public class UserRepository : IUserRepository
    {

        public readonly string _connectionString;

        public UserRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private MySqlConnection GetConnection() => new MySqlConnection(_connectionString);

        public async Task<User?> GetByEmailAsync(string email)
        {
            using var connection = GetConnection();
            var sql = @"SELECT 
                            u.id,
                            u.email,
                            u.passwordHash,
                            u.roleId ,
                            u.createdAt,
                            r.id as RoleId, 
                            r.name as RoleName 
                        FROM 
                            users u
                        JOIN 
                            roles r ON u.roleId = r.id
                        WHERE 
                            u.email = @Email";
            var result = await connection.QueryAsync<User, Role, User>(
                sql,
                (user, role) => { user.Role = role; return user; },
                new { Email = email },
                splitOn: "RoleId"
            );
            return result.FirstOrDefault();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            var sql = @"SELECT 
                            u.*, 
                            r.id as RoleId, 
                            r.name as RoleName 
                    FROM 
                        users u
                    JOIN 
                        roles r ON u.roleId = r.id 
                    WHERE 
                        u.id = @Id";
            var result = await connection.QueryAsync<User, Role, User>(
                sql,
                (user, role) => { user.Role = role; return user; },
                new { Id = id },
                splitOn: "RoleId"
            );
            return result.FirstOrDefault();
        }

        public async Task<int> CreateUserAsync(User user)
        {
            using var connection = GetConnection();
            var sql = "INSERT INTO users (email, passwordHash, roleId) VALUES (@Email, @PasswordHash, @RoleId);";
            await connection.ExecuteAsync(sql, user);

            var id = await connection.ExecuteScalarAsync<int>("SELECT LAST_INSERT_ID();");
            return id;
        }
    }
}
