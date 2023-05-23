using TechShop.Dtos.Category;

namespace TechShop.BLL.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
    }
}
