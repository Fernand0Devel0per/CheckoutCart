using TechShop.Domain;
using TechShop.Helpers.Enums;

namespace TechShop.DAL.Interface
{
    public interface IStatusDao
    {
        Task<Status> FindByCodeAsync(StatusCode code);
        Task<IEnumerable<Status>> FindAllAsync();
        Task<Status> GetByIdAsync(Guid id);
    }
}
