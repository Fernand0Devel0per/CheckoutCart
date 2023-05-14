using CheckoutCart.Domain;
using CheckoutCart.Helpers.Enums;

namespace CheckoutCart.DAL.Interface
{
    public interface IStatusDao
    {
        Task<Status> FindByCodeAsync(StatusCode code);
        Task<IEnumerable<Status>> FindAllAsync();
    }
}
