using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Helpers.Data;
using CheckoutCart.Helpers.Enums;
using System.Data.SqlClient;

namespace CheckoutCart.DAL
{
    public class CategoryDao : ICategoryDao
    {
        private const string ColumnId = "Id";
        private const string ColumnCode = "Code";
        private const string ColumnName = "Name";

        private readonly string _connectionString;
        public CategoryDao(IConfiguration configuration)
        {
            _connectionString = configuration.GetDefaultConnectionString();
        }

        public async Task<Category> FindByIdAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Categories WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Category
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    Code = reader.GetInt32(reader.GetOrdinal(ColumnCode)),
                    Name = reader.GetString(reader.GetOrdinal(ColumnName)),
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<Category> FindByCodeAsync(CategoryCode code)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Categories WHERE Code = @code";
            command.Parameters.AddWithValue("@code", code);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Category
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    Code = reader.GetInt32(reader.GetOrdinal(ColumnCode)),
                    Name = reader.GetString(reader.GetOrdinal(ColumnName)),
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<IEnumerable<Category>> FindAllAsync()
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM Categories";

            var categories = new List<Category>();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var status = new Category
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    Code = reader.GetInt32(reader.GetOrdinal(ColumnCode)),
                    Name = reader.GetString(reader.GetOrdinal(ColumnName))
                };

                categories.Add(status);
            }
            return categories;
        }
    }
}
