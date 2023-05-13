using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Helpers.Data;
using System.Data.SqlClient;

namespace CheckoutCart.DAL
{
    public class RoleDao : IRoleDao
    {
        private const string ColumnId = "Id";
        private const string ColumnName = "Name";

        private readonly string _connectionString;

        public RoleDao(IConfiguration configuration)
        {
            _connectionString = configuration.GetDefaultConnectionString();
        }

        public async Task<Role> FindByIdAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Roles WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Role
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    Name = reader.GetString(reader.GetOrdinal(ColumnName))
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<Role> FindByNameAsync(string roleName)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Roles WHERE Name = @name";
            command.Parameters.AddWithValue("@name", roleName);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Role
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    Name = reader.GetString(reader.GetOrdinal(ColumnName))
                };
            }
            else
            {
                return null;
            }
        }
    }
}

