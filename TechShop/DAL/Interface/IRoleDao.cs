using TechShop.Domain;

namespace TechShop.DAL.Interface
{
    public interface IRoleDao
    {
        Task<Role> FindByIdAsync(Guid id);
        Task<Role> FindByNameAsync(string roleName);
    }
}
