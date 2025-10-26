using Dapper;
using MySqlConnector;
using Prueba02JWT.Models;

namespace Prueba02JWT.Repository
{
    public class RoleRepository : IRoleRepository
    {
        private readonly string _connectionString;

        public RoleRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        private MySqlConnection GetConnection() => new MySqlConnection(_connectionString);

        public async Task<Role?> GetByNameAsync(string name)
        {
            using var connection = GetConnection();
            var sql = "SELECT * FROM roles WHERE name = @Name";
            return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Name = name });
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            using var connection = GetConnection();
            var sql = "SELECT * FROM roles WHERE id = @Id";
            return await connection.QueryFirstOrDefaultAsync<Role>(sql, new { Id = id });
        }
    }
}
