using AutoMapper;
using CheckoutCart.BLL.Interface;
using CheckoutCart.DAL.Interface;
using CheckoutCart.Dtos.Category;

namespace CheckoutCart.BLL
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDao _categoryDao;
        private readonly IMapper _mapper;

        public CategoryService(ICategoryDao categoryDao, IMapper mapper)
        {
            _categoryDao = categoryDao;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CategoryResponse>> GetAllCategoriesAsync()
        {
            var allCategories = await _categoryDao.FindAllAsync();
            return _mapper.Map<IEnumerable<CategoryResponse>>(allCategories);
        }
    }
}
