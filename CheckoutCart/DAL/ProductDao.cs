using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Dtos.Common;
using CheckoutCart.Helpers.Data;
using CheckoutCart.Helpers.Enums;
using CheckoutCart.Helpers.Exceptions;
using CheckoutCart.Helpers.Security.Contants;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace CheckoutCart.DAL
{
    public class ProductDao : IProductDao
    {

        private const string ColumnId = "Id";
        private const string ColumnName = "Name";
        private const string ColumnPrice = "Price";
        private const string ColumnDescription = "Description";
        private const string ColumnCategoryId = "CategoryId";
        private const string ColumnIsActive = "IsActive";

        private readonly string _connectionString;

        public ProductDao(IConfiguration configuration)
        {
            _connectionString = configuration.GetDefaultConnectionString();
        }


        public async Task<Product> CreateAsync(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO PRODUCTS (Id, Name, Price, Description, CategoryId, IsActive) OUTPUT INSERTED.ID VALUES (NEWGUID, @name, @price, @description, @categoryId, @isActive)";
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@price", product.Price);
            command.Parameters.AddWithValue("@description", product.Description);
            command.Parameters.AddWithValue("@categoryId", product.CategoryId);
            command.Parameters.AddWithValue("@isActive", product.IsActive);

            product.Id = (Guid)await command.ExecuteScalarAsync();

            return product;
        }

        public async Task<bool> UpdateAsync(Product product)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "Update PRODUCTS SET Name = @name, Price = @price, Description = @description, CategoryId = @categoryId, IsActive = @categoryId WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", product.Id);
            command.Parameters.AddWithValue("@name", product.Name);
            command.Parameters.AddWithValue("@price", product.Price);
            command.Parameters.AddWithValue("@description", product.Description);
            command.Parameters.AddWithValue("@categoryId", product.CategoryId);
            command.Parameters.AddWithValue("@isActive", product.IsActive);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.OpenAsync();

                using var command = connection.CreateCommand();
                command.CommandText = "DELETE FROM Products WHERE Id = @id";
                command.Parameters.AddWithValue("@id", id);

                return await command.ExecuteNonQueryAsync() > 0;
            }
            catch (SqlException ex) when (ex.Number == 547)
            {
                throw new ProductInUseException();
            }
        }

        public async Task<bool> ToggleActiveStatusAsync(bool isActive, Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();

            command.CommandText = "Update PRODUCTS SET IsActive = @isActive WHERE Id = @id ";
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@isActive", isActive);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<Product> GetProductByIdAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @"
                    SELECT Id, Name, Price, Description, CategoryId, IsActive 
                    FROM PRODUCTS 
                    WHERE Id = @id;";

            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();

            if (await reader.ReadAsync())
            {
                var product = new Product
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    Name = reader.GetString(reader.GetOrdinal(ColumnName)),
                    Price = reader.GetDecimal(reader.GetOrdinal(ColumnPrice)),
                    Description = reader.IsDBNull(reader.GetOrdinal(ColumnDescription))
                        ? string.Empty : reader.GetString(reader.GetOrdinal(ColumnDescription)),
                    CategoryId = reader.GetGuid(reader.GetOrdinal(ColumnCategoryId)),
                    IsActive = reader.GetBoolean(reader.GetOrdinal(ColumnIsActive))
                };

                return product;
            }

            return null;
        }

        public async Task<PagedResult<Product>> GetProductsAsync(int page = 1, int pageSize = 10, bool onlyActive = false)
        {

            ValidatePageSize(pageSize);

             var result = new PagedResult<Product>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @$"
                    SELECT COUNT(*) FROM PRODUCTS { (onlyActive ? "WHERE IsActive = 1" : "")};
                    SELECT* FROM PRODUCTS { (onlyActive ? "WHERE IsActive = 1" : "")}
                    ORDER BY Name OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            CalculatePagingParameters(page, pageSize, command);

            using var reader = await command.ExecuteReaderAsync();

            await CalculatePageData(reader, result, pageSize);

            await ReadProducts(reader, result);

            return result;
        }

        public async Task<PagedResult<Product>> GetProductsByCategoryAsync(Guid categoryId, int page = 1, int pageSize = 10, bool onlyActive = false)
        {

            ValidatePageSize(pageSize);

            var result = new PagedResult<Product>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @$"
                SELECT COUNT(*) FROM PRODUCTS WHERE CategoryId = @categoryId{(onlyActive ? " AND IsActive = 1" : "")};
                SELECT * FROM PRODUCTS WHERE CategoryId = @categoryId{(onlyActive ? " AND IsActive = 1" : "")}
                ORDER BY Name OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY";
            command.Parameters.AddWithValue($"@categoryId", categoryId);
            CalculatePagingParameters(page, pageSize, command);

            using var reader = await command.ExecuteReaderAsync();

            await CalculatePageData(reader, result, pageSize);

            await ReadProducts(reader, result);

            return result;
        }

        private void CalculatePagingParameters(int page, int pageSize, SqlCommand command)
        {
            command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
            command.Parameters.AddWithValue("@pageSize", pageSize);
        }

        private async Task CalculatePageData(SqlDataReader reader, PagedResult<Product> result, int pageSize)
        {
            if (reader.Read())
            {
                result.TotalItems = reader.GetInt32(0);
                result.TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);
            }
        }

        private async Task ReadProducts(SqlDataReader reader, PagedResult<Product> result)
        {
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var product = new Product
                    {
                        Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                        Name = reader.GetString(reader.GetOrdinal(ColumnName)),
                        Price = reader.GetDecimal(reader.GetOrdinal(ColumnPrice)),
                        Description = reader.IsDBNull(reader.GetOrdinal(ColumnDescription))
                        ? string.Empty : reader.GetString(reader.GetOrdinal(ColumnDescription)),
                        CategoryId = reader.GetGuid(reader.GetOrdinal(ColumnCategoryId)),
                        IsActive = reader.GetBoolean(reader.GetOrdinal(ColumnIsActive))
                    };


                    result.Items.Add(product);
                }
            }

        }

        private void ValidatePageSize(int pageSize)
        {
            if (pageSize < 1 || pageSize > 100)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be between 1 and 100.");
            }
        }
    }
}
