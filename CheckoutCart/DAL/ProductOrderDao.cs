using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Helpers.Data;
using System.Data.SqlClient;

namespace CheckoutCart.DAL
{
    public class ProductOrderDao : IProductOrderDao
    {
        private const string ColumnProductId = "ProductId";
        private const string ColumnOrderId = "OrderId";
        private const string ColumnQuantity = "Quantity";
        private const string ColumnPriceAtOrder = "PriceAtOrder";

        private readonly string _connectionString;

        public ProductOrderDao(IConfiguration configuration)
        {
            _connectionString = _connectionString = configuration.GetDefaultConnectionString();
        }

        public async Task<bool> AddProductToOrderAsync(ProductOrder productOrder)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "INSERT INTO ProductOrders (ProductId, OrderId, Quantity, PriceAtOrder) VALUES (@ProductId, @OrderId, @Quantity, @PriceAtOrder)";
            command.Parameters.AddWithValue("@ProductId", productOrder.ProductId);
            command.Parameters.AddWithValue("@OrderId", productOrder.OrderId);
            command.Parameters.AddWithValue("@Quantity", productOrder.Quantity);
            command.Parameters.AddWithValue("@PriceAtOrder", productOrder.PriceAtOrder);

            return await command.ExecuteNonQueryAsync() == 1;
        }

        public async Task<bool> UpdateProductQuantityInOrderAsync(Guid orderId, Guid productId, int newQuantity)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "UPDATE ProductOrders SET Quantity = @NewQuantity WHERE OrderId = @OrderId AND ProductId = @ProductId";
            command.Parameters.AddWithValue("@NewQuantity", newQuantity);
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@ProductId", productId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> RemoveProductFromOrderAsync(Guid orderId, Guid productId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "DELETE FROM ProductOrders WHERE OrderId = @OrderId AND ProductId = @ProductId";
            command.Parameters.AddWithValue("@OrderId", orderId);
            command.Parameters.AddWithValue("@ProductId", productId);

            return await command.ExecuteNonQueryAsync() > 0;
        }

        public async Task<IEnumerable<ProductOrder>> GetAllProductsInOrderAsync(Guid orderId)
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ProductOrders WHERE OrderId = @OrderId";
            command.Parameters.AddWithValue("@OrderId", orderId);

            var productOrders = new List<ProductOrder>();

            using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var productOrder = new ProductOrder
                {
                    ProductId = reader.GetGuid(reader.GetOrdinal(ColumnProductId)),
                    OrderId = reader.GetGuid(reader.GetOrdinal(ColumnOrderId)),
                    Quantity = reader.GetInt32(reader.GetOrdinal(ColumnQuantity)),
                    PriceAtOrder = reader.GetDecimal(reader.GetOrdinal(ColumnPriceAtOrder))
                };

                productOrders.Add(productOrder);
            }

            return productOrders;
        }

    }
}
