using TechShop.Domain;
using TechShop.Dtos.Common;

namespace TechShop.DAL.Interface
{
    public interface IOrderDao
    {
        Task<Order> CreateAsync(Order order);
        Task<bool> UpdateStatusAsync(Guid id, Guid statusId);
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<PagedResult<Order>> GetOrdersByUserAsync(Guid userId, int page = 1, int pageSize = 10);
        Task<bool> DoesUserHaveOpenOrderAsync(Guid userId);
    }
}
