using AutoMapper;
using CheckoutCart.BLL.Interface;
using CheckoutCart.DAL.Interface;
using CheckoutCart.Domain;
using CheckoutCart.Dtos.Common;
using CheckoutCart.Dtos.Product;
using CheckoutCart.Helpers.Enums;
using CheckoutCart.Helpers.Exceptions;

namespace CheckoutCart.BLL
{
    public class ProductService : IProductService
    {
        private readonly IProductDao _productDao;
        private readonly ICategoryDao _categoryDao;
        private readonly IMapper _mapper;

        public ProductService(IProductDao productDao, IMapper mapper, ICategoryDao categoryDao)
        {
            _productDao = productDao;
            _mapper = mapper;
            _categoryDao = categoryDao;
        }

        public async Task<ProductCreateResponse> CreateProductAsync(ProductCreateRequest productRequest)
        {
            var category = await _categoryDao.FindByCodeAsync(productRequest.Category);
            if (category is null)
            {
                throw new ArgumentException($"Category with code {productRequest.Category} not found. Product creation failed as valid category is required.");
            }

            var newProduct = _mapper.Map<Product>(productRequest);
            newProduct.CategoryId = category.Id;
            var productCreated = await _productDao.CreateAsync(newProduct);
            var productResponse = _mapper.Map<ProductCreateResponse>(productCreated);
            return productResponse;
        }

        public async Task<bool> UpdateProductAsync(Guid id, ProductUpdateRequest productRequest)
        {
            var product = await _productDao.GetProductByIdAsync(id);
            if (product is null)
            {
                throw new ProductNotFoundException($"Product with Id {id} could not be found when attempting to update.");
            }
            var category = await _categoryDao.FindByCodeAsync(productRequest.Category);
            if (category is null)
            {
                throw new ArgumentException($"Category with code {productRequest.Category} not found. Update failed as category is required for product.");
            }
            product = _mapper.Map<Product>(productRequest);
            product.Id = id;
            product.CategoryId = category.Id;

            return await _productDao.UpdateAsync(product);
        }

        public async Task<bool> DeleteProductAsync(Guid id)
        {
            return await _productDao.DeleteAsync(id);
        }

        public async Task<bool> ToggleProductActiveStatusAsync(bool isActive, Guid id)
        {
            return await _productDao.ToggleActiveStatusAsync(isActive, id);
        }

        public async Task<ProductResponse> GetProductByIdAsync(Guid id)
        {
            var product = await _productDao.GetProductByIdAsync(id);
            if (product is null)
            {
                throw new ProductNotFoundException($"Product with Id {id} does not exist in the database.");
            }
            var category = await _categoryDao.FindByIdAsync(product.CategoryId);
            var productResponse = _mapper.Map<ProductResponse>(product);
            productResponse.Category = category.Name;

            return productResponse;
        }

        public async Task<PagedResult<ProductResponse>> GetProductsAsync(int page = 1, int pageSize = 10, bool onlyActive = false)
        {

            var pagedProducts = await _productDao.GetProductsAsync(page, pageSize, onlyActive);
            var pagedProductResponses = new PagedResult<ProductResponse>
            {
                TotalItems = pagedProducts.TotalItems,
                TotalPages = pagedProducts.TotalPages,
            };

            foreach (var product in pagedProducts.Items)
            {
                var category = await _categoryDao.FindByIdAsync(product.CategoryId);
                var productResponse = _mapper.Map<ProductResponse>(product);
                productResponse.Category = category.Name;
                pagedProductResponses.Items.Add(productResponse);
            }

            return pagedProductResponses;
        }

        public async Task<PagedResult<ProductResponse>> GetProductsByCategoryAsync(CategoryCode categoryCode, int page = 1, int pageSize = 10, bool onlyActive = false)
        {
            var category = await _categoryDao.FindByCodeAsync(categoryCode);
            if (category is null)
            {
                throw new ArgumentException($"Category with code {categoryCode} not found. Update failed as category is required for product.");
            }

            var pagedProducts = await _productDao.GetProductsByCategoryAsync(category.Id, page, pageSize, onlyActive);
            var pagedProductResponses = new PagedResult<ProductResponse>
            {
                TotalItems = pagedProducts.TotalItems,
                TotalPages = pagedProducts.TotalPages,
            };

            foreach (var product in pagedProducts.Items)
            {
                var productResponse = _mapper.Map<ProductResponse>(product);
                productResponse.Category = category.Name;
                pagedProductResponses.Items.Add(productResponse);
            }

            return pagedProductResponses;

            
        }
    }
}
