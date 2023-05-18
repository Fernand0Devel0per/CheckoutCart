using CheckoutCart.Domain;
using CheckoutCart.Dtos.Common;

namespace CheckoutCart.DAL.Interface
{
    public interface IOrderDao
    {
        Task<Order> CreateAsync(Order order);
        Task UpdateStatusAsync(Guid id, Guid statusId);
        Task<Order> GetOrderByIdAsync(Guid id);
        Task<PagedResult<Order>> GetOrdersByUserAsync(Guid userId, int page = 1, int pageSize = 10);
    }
}
