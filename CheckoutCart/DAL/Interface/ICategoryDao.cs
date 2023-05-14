using CheckoutCart.Domain;
using CheckoutCart.Helpers.Enums;

namespace CheckoutCart.DAL.Interface
{
    public interface ICategoryDao
    {
        Task<Category> FindByIdAsync(Guid id);
        Task<Category> FindByCodeAsync(CategoryCode code);
        Task<IEnumerable<Category>> FindAllAsync();
    }
}
