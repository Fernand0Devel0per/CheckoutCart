using CheckoutCart.Dtos.Common;
using CheckoutCart.Dtos.Product;
using CheckoutCart.Helpers.Enums;

namespace CheckoutCart.BLL.Interface
{
    public interface IProductService
    {
        Task<ProductCreateResponse> CreateProductAsync(ProductCreateRequest productRequest);
        Task<bool> UpdateProductAsync(Guid id, ProductUpdateRequest productRequest);
        Task<bool> DeleteProductAsync(Guid id);
        Task<bool> ToggleProductActiveStatusAsync(bool isActive, Guid id);
        Task<ProductResponse> GetProductByIdAsync(Guid id);
        Task<PagedResult<ProductResponse>> GetProductsAsync(int page = 1, int pageSize = 10, bool onlyActive = false);
        Task<PagedResult<ProductResponse>> GetProductsByCategoryAsync(CategoryCode categoryCode, int page = 1, int pageSize = 10, bool onlyActive = false);
    }
}
