using TechShop.Dtos.Common;
using TechShop.Dtos.Order;
using TechShop.Helpers.Enums;

namespace TechShop.BLL.Interface
{
    public interface IOrderService
    {
        Task<OrderCreateResponse> CreateAsync(Guid userId);
        Task<bool> UpdateStatusAsync(Guid id, StatusCode code);
        Task<OrderSearchResponse> GetOrderByIdAsync(Guid id);
        Task<OrderWithProductResponse> GetOrderByIdWithProductsAsync(Guid id);
        Task<PagedResult<OrderSearchResponse>> GetOrdersByUserAsync(Guid userId, int page = 1, int pageSize = 10);
        Task<bool> AddProductToOrderAsync(Guid orderId, Guid productId, int quantity);
        Task<bool> UpdateProductQuantityInOrderAsync(Guid orderId, Guid productId, int newQuantity);
        Task<bool> RemoveProductFromOrderAsync(Guid orderId, Guid productId);
    }
}
