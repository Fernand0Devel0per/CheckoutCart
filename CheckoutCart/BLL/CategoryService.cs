using AutoMapper;
using TechShop.BLL.Interface;
using TechShop.DAL.Interface;
using TechShop.Dtos.Category;

namespace TechShop.BLL
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
