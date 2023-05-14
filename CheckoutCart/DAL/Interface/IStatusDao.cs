using CheckoutCart.Domain;
using CheckoutCart.Helpers.Enums;

namespace CheckoutCart.DAL.Interface
{
    public interface IStatusDao
    {
        Task<Status> FindByCodeAsync(StatusEnum code);
        Task<IEnumerable<Status>> FindAllAsync();
    }
}
