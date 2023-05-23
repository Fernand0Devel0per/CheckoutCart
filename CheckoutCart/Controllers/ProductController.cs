using CheckoutCart.BLL.Interface;
using CheckoutCart.Dtos.Product;
using CheckoutCart.Helpers.Enums;
using CheckoutCart.Helpers.Exceptions;
using CheckoutCart.Helpers.Security.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CheckoutCart.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="productRequest">The product to be created.</param>
        /// <returns>The created product.</returns>
        /// <response code="201">If the product is successfully created.</response>
        /// <response code="400">If the product request is null or invalid.</response>
        /// <response code="500">If there is an internal server error.</response>
        ///  /// <remarks>
        /// Sample request:
        ///
        ///     POST /Product
        ///     {
        ///        "name": "Product 1",
        ///        "category": "Category 1",
        ///        "price": 100.0,
        ///        "description": "This is a sample product",
        ///        "isActive": true
        ///     }
        /// </remarks>
        [Authorize(Roles = $"{Roles.Employer},{Roles.Admin}")]
        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductCreateRequest productRequest)
        {
            try
            {
                var response = await _productService.CreateProductAsync(productRequest);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (ArgumentException ex)
            {
                return BadRequest( new {Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while creating the product. Please try again later." });
            }
        }

        /// <summary>
        /// Updates an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to be updated.</param>
        /// <param name="productRequest">The updated product.</param>
        /// <returns>A NoContent response if the product was updated.</returns>
        /// <response code="204">If the product was updated successfully.</response>
        /// <response code="400">If the product is null.</response>
        /// <response code="404">If the product could not be found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Product/{id}
        ///     {
        ///        "name": "Updated Product 1",
        ///        "category": "Updated Category 1",
        ///        "price": 120.0,
        ///        "description": "This is an updated product",
        ///        "isActive": true
        ///     }
        /// </remarks>
        [Authorize(Roles = $"{Roles.Employer},{Roles.Admin}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(Guid id, ProductUpdateRequest productRequest)
        {

            try
            {
                var updated = await _productService.UpdateProductAsync(id, productRequest);

                if (!updated)
                {
                    return NotFound(new { Message = $"Product with Id {id} could not be found when attempting to update." });
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while updating the product." });
            }
        }

        /// <summary>
        /// Toggles the active status of a specific product.
        /// </summary>
        /// <param name="id">The ID of the product to toggle the status.</param>
        /// <param name="isActive">The updated active status of the product.</param>
        /// <returns>A NoContent response if the product's active status was updated.</returns>
        /// <response code="204">If the product's active status was updated successfully.</response>
        /// <response code="404">If the product could not be found.</response>
        /// <response code="500">If an internal server error occurs.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Product/{id}/status
        ///     {
        ///        "isActive": false
        ///     }
        /// </remarks>
        [Authorize(Roles = $"{Roles.Employer},{Roles.Admin}")]
        [HttpPut("{id}/status")]
        public async Task<IActionResult> ToggleProductStatus(Guid id, [FromBody] bool isActive)
        {
            try
            {
                var updated = await _productService.ToggleProductActiveStatusAsync(isActive, id);

                if (!updated)
                {
                    return NotFound(new { Message = $"Product with Id {id} could not be found." });
                }

                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while updating the product's status." });
            }
        }

        /// <summary>
        /// Deletes a specific product.
        /// </summary>
        /// <param name="id">The ID of the product to be deleted.</param>
        /// <returns>A NoContent response if the product was deleted.</returns>
        /// <response code="204">If the product was deleted successfully.</response>
        /// <response code="404">If the product could not be found.</response>
        /// <response code="409">If the product is in use and cannot be deleted.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize(Roles = $"{Roles.Employer},{Roles.Admin}")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            try
            {
                var deleted = await _productService.DeleteProductAsync(id);

                if (!deleted)
                {
                    return NotFound(new { Message = $"Product with Id {id} could not be found." });
                }

                return NoContent();
            }
            catch (ProductInUseException)
            {
                return Conflict(new { Message = "Product is in use and cannot be deleted." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An unexpected error occurred while deleting the product." });
            }
        }

        /// <summary>
        /// Retrieves all products.
        /// </summary>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="onlyActive">Flag to filter only active products.</param>
        /// <returns>A list of Products.</returns>
        /// <response code="200">Returns a list of products.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            try
            {
                var product = await _productService.GetProductByIdAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (ProductNotFoundException)
            {
                return NotFound(new { Message = $"Product with Id {id} does not exist in the database." });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving the product." });
            }
        }

        /// <summary>
        /// Retrieves all products by category.
        /// </summary>
        /// <param name="categoryCode">The category code.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="onlyActive">Flag to filter only active products.</param>
        /// <returns>A list of Products in the specified category.</returns>
        /// <response code="200">Returns a list of products in the specified category.</response>
        /// <response code="400">If the category code is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetProducts(int page = 1, int pageSize = 10, bool onlyActive = false)
        {
            try
            {
                var pagedProducts = await _productService.GetProductsAsync(page, pageSize, onlyActive);
                return Ok(pagedProducts);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving the products." });
            }
        }

        /// <summary>
        /// Retrieves all products by category.
        /// </summary>
        /// <param name="categoryCode">The category code.</param>
        /// <param name="page">Page number for pagination.</param>
        /// <param name="pageSize">Number of items per page.</param>
        /// <param name="onlyActive">Flag to filter only active products.</param>
        /// <returns>A list of Products in the specified category.</returns>
        /// <response code="200">Returns a list of products in the specified category.</response>
        /// <response code="400">If the category code is invalid.</response>
        /// <response code="500">If an internal server error occurs.</response>
        [Authorize]
        [HttpGet("category/{categoryCode}")]
        public async Task<IActionResult> GetProductsByCategory(CategoryCode categoryCode, int page = 1, int pageSize = 10, bool onlyActive = false)
        {
            try
            {
                var pagedProducts = await _productService.GetProductsByCategoryAsync(categoryCode, page, pageSize, onlyActive);
                return Ok(pagedProducts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while retrieving the products." });
            }
        }
    }
}
