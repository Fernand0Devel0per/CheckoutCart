using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Helpers.Data;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;

namespace CheckoutCart.DAL
{
    public class UserDao : IUserDao
    {
        private const string ColumnId = "Id";
        private const string ColumnUsername = "Username";
        private const string ColumnPassword = "Password";
        private const string ColumnRoleId = "RoleId";

        private readonly string _connectionString;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserDao(IConfiguration configuration, IPasswordHasher<User> passwordHasher)
        {
            _connectionString = configuration.GetDefaultConnectionString();
            _passwordHasher = passwordHasher;
        }

        public async Task<User> FindByUsernameAsync(string username)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Username = @username";
            command.Parameters.AddWithValue("@username", username);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    Username = reader.GetString(reader.GetOrdinal(ColumnUsername)),
                    Password = reader.GetString(reader.GetOrdinal(ColumnPassword)),
                    RoleId = reader.GetGuid(reader.GetOrdinal(ColumnRoleId)),
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<User> FindByIdAsync(Guid userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Users WHERE Id = @id";
            command.Parameters.AddWithValue("@id", userId);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new User
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    Username = reader.GetString(reader.GetOrdinal(ColumnUsername)),
                    Password = reader.GetString(reader.GetOrdinal(ColumnPassword)),
                    RoleId = reader.GetGuid(reader.GetOrdinal(ColumnRoleId)),
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<User> CreateAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var hashedPassword = _passwordHasher.HashPassword(user, user.Password);

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO Users (Id, Username, Password, RoleId) OUTPUT INSERTED.ID VALUES (NEWID(), @username, @password, @roleId)";
            command.Parameters.AddWithValue("@username", user.Username);
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@roleId", user.RoleId);

            user.Id = (Guid)await command.ExecuteScalarAsync();
            return user;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            var hashedPassword = _passwordHasher.HashPassword(user, user.Password);

            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE Users SET Password = @password, RoleId = @roleId WHERE Id = @id";
            command.Parameters.AddWithValue("@password", hashedPassword);
            command.Parameters.AddWithValue("@roleId", user.RoleId);
            command.Parameters.AddWithValue("@id", user.Id);

            return await command.ExecuteNonQueryAsync() > 0;
        }
    }
}
