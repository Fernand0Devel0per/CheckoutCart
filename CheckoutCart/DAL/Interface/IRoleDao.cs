using CheckoutCart.Domain;

namespace CheckoutCart.DAL.Interface
{
    public interface IRoleDao
    {
        Task<Role> FindByIdAsync(Guid id);
        Task<Role> FindByNameAsync(string roleName);
    }
}
