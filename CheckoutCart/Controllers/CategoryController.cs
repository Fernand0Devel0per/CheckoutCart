using CheckoutCart.BLL;
using CheckoutCart.BLL.Interface;
using CheckoutCart.Helpers.Security.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutCart.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retrieves all categories.
        /// </summary>
        /// <returns>The list of categories.</returns>
        /// <response code="200">Returns the list of categories.</response>
        /// <response code="500">Internal server error.</response>
        [Authorize(Roles = $"{Roles.Employer},{Roles.Admin}" )]
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            try
            {
                var categoriesList = await _categoryService.GetAllCategoriesAsync();
                return Ok(categoriesList);
            }
            catch (Exception) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred." });
            }
        }
    }
}
