using TechShop.DAL.Interface;
using TechShop.Domain;
using TechShop.Dtos.Common;
using TechShop.Helpers.Data;
using TechShop.Helpers.Enums;
using System.Data.SqlClient;

namespace TechShop.DAL
{
    public class OrderDao : IOrderDao
    {
        private const string ColumnId = "Id";
        private const string ColumnOrderDate = "OrderDate";
        private const string ColumnUserId = "UserId";
        private const string ColumnStatusId = "StatusId";

        public Guid StatusId { get; set; }

        private readonly string _connectionString;

        public OrderDao(IConfiguration configuration)
        {
            _connectionString = configuration.GetDefaultConnectionString();
        }

        public async Task<Order> CreateAsync(Order order)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO ORDERS (Id, OrderDate, UserId, StatusId) OUTPUT INSERTED.ID VALUES (NEWID(), @orderDate, @userId, @statusId)";
            command.Parameters.AddWithValue("@orderDate", DateTime.UtcNow);
            command.Parameters.AddWithValue("@userId", order.UserId);
            command.Parameters.AddWithValue("@statusId", order.StatusId);
            
            order.OrderDate = DateTime.UtcNow;
            order.Id = (Guid)await command.ExecuteScalarAsync();

            return order;
        }


        public async Task<bool> UpdateStatusAsync(Guid id, Guid statusId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = $"UPDATE ORDERS SET {ColumnStatusId} = @statusId WHERE {ColumnId} = @id";
            command.Parameters.AddWithValue("@statusId", statusId);
            command.Parameters.AddWithValue("@id", id);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<Order> GetOrderByIdAsync(Guid id)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT * FROM ORDERS WHERE Id = @id";
            command.Parameters.AddWithValue("@id", id);

            using var reader = await command.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                return new Order
                {
                    Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                    OrderDate = reader.GetDateTime(reader.GetOrdinal(ColumnOrderDate)),
                    UserId = reader.GetGuid(reader.GetOrdinal(ColumnUserId)),
                    StatusId = reader.GetGuid(reader.GetOrdinal(ColumnStatusId))
                };
            }

            return null;
        }

        public async Task<bool> DoesUserHaveOpenOrderAsync(Guid userId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @$"
                SELECT COUNT(*) FROM ORDERS 
                INNER JOIN STATUSES ON ORDERS.StatusId = STATUSES.Id 
                WHERE ORDERS.UserId = @userId AND STATUSES.Code = @statusCode";

            command.Parameters.AddWithValue("@userId", userId);
            command.Parameters.AddWithValue("@statusCode", (int)StatusCode.Open);

            var count = (int)await command.ExecuteScalarAsync();

            return count > 0;
        }

        public async Task<PagedResult<Order>> GetOrdersByUserAsync(Guid userId, int page = 1, int pageSize = 10)
        {
            ValidatePageSize(pageSize);

            var result = new PagedResult<Order>();

            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = @$"
                SELECT COUNT(*) FROM ORDERS WHERE UserId = @userId;
                SELECT * FROM ORDERS WHERE UserId = @userId
                ORDER BY OrderDate OFFSET @offset ROWS FETCH NEXT @pageSize ROWS ONLY;";

            CalculatePagingParameters(page, pageSize, command);

            command.Parameters.AddWithValue("@userId", userId);

            using var reader = await command.ExecuteReaderAsync();

            await CalculatePageData(reader, result, pageSize);

            await ReadOrders(reader, result);

            return result;
        }

        private async Task ReadOrders(SqlDataReader reader, PagedResult<Order> result)
        {
            if (reader.NextResult())
            {
                while (reader.Read())
                {
                    var order = new Order
                    {
                        Id = reader.GetGuid(reader.GetOrdinal(ColumnId)),
                        OrderDate = reader.GetDateTime(reader.GetOrdinal(ColumnOrderDate)),
                        UserId = reader.GetGuid(reader.GetOrdinal(ColumnUserId)),
                        StatusId = reader.GetGuid(reader.GetOrdinal(ColumnStatusId))
                    };

                    result.Items.Add(order);
                }
            }
        }

        private void CalculatePagingParameters(int page, int pageSize, SqlCommand command)
        {
            command.Parameters.AddWithValue("@offset", (page - 1) * pageSize);
            command.Parameters.AddWithValue("@pageSize", pageSize);
        }

        private async Task CalculatePageData(SqlDataReader reader, PagedResult<Order> result, int pageSize)
        {
            if (reader.Read())
            {
                result.TotalItems = reader.GetInt32(0);
                result.TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);
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
