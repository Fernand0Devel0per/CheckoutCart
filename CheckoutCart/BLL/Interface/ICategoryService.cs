using CheckoutCart.Dtos.Category;

namespace CheckoutCart.BLL.Interface
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync();
    }
}
