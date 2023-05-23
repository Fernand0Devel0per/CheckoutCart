using TechShop.Domain;
using TechShop.Helpers.Enums;

namespace TechShop.DAL.Interface
{
    public interface ICategoryDao
    {
        Task<Category> FindByIdAsync(Guid id);
        Task<Category> FindByCodeAsync(CategoryCode code);
        Task<IEnumerable<Category>> FindAllAsync();
    }
}
