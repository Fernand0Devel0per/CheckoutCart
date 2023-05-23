using TechShop.BLL.Interface;
using TechShop.Dtos.Order;
using TechShop.Helpers.Enums;
using TechShop.Helpers.Exceptions;
using TechShop.Helpers.Security.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        /// <summary>
        /// Creates a new order for a user.
        /// </summary>
        /// <param name="userId">The user id for which the order is to be created.</param>
        /// <returns>The created order.</returns>
        /// <response code="201">If the order is successfully created.</response>
        /// <response code="400">If the user already has an open order or if the status is not found.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Order
        ///     {
        ///        "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
        ///     }
        /// </remarks>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAsync(Guid userId)
        {
            try
            {
                var response = await _orderService.CreateAsync(userId);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (OpenOrderExistsException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Updates the status of an order.
        /// </summary>
        /// <param name="id">The id of the order to be updated.</param>
        /// <param name="code">The new status code.</param>
        /// <returns>No content.</returns>
        /// <response code="204">If the order's status is successfully updated.</response>
        /// <response code="400">If the status transition is invalid or if the status is not found.</response>
        /// <response code="404">If the order is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Order/{id}
        ///     {
        ///        "code": 2
        ///     }
        /// </remarks>
        [Authorize(Roles = $"{Roles.Employer},{Roles.Admin}")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStatusAsync(Guid id, StatusCode code)
        {
            try
            {
                var result = await _orderService.UpdateStatusAsync(id, code);

                if (result)
                    return NoContent();

                return BadRequest(new { Message = "Unable to update order status" });
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (StatusNotFoundException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidStatusTransitionException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Retrieves an order by its id.
        /// </summary>
        /// <param name="id">The id of the order to be retrieved.</param>
        /// <returns>The order.</returns>
        /// <response code="200">If the order is successfully retrieved.</response>
        /// <response code="404">If the order is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Order/{id}
        /// </remarks>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOrderByIdAsync(Guid id)
        {
            try
            {
                var response = await _orderService.GetOrderByIdAsync(id);
                return Ok(response);
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Retrieves an order by its id, including its associated products.
        /// </summary>
        /// <param name="id">The id of the order to be retrieved.</param>
        /// <returns>The order with its associated products.</returns>
        /// <response code="200">If the order is successfully retrieved.</response>
        /// <response code="404">If the order is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Order/{id}/products
        /// </remarks>
        [Authorize]
        [HttpGet("{id}/products")]
        public async Task<IActionResult> GetOrderByIdWithProductsAsync(Guid id)
        {
            try
            {
                var response = await _orderService.GetOrderByIdWithProductsAsync(id);
                return Ok(response);
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Retrieves all orders associated with a user.
        /// </summary>
        /// <param name="userId">The id of the user.</param>
        /// <param name="page">The page number (default is 1).</param>
        /// <param name="pageSize">The page size (default is 10).</param>
        /// <returns>The orders associated with the user.</returns>
        /// <response code="200">If the orders are successfully retrieved.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /Order/user/{userId}?page=1&amp;pageSize=10
        /// </remarks>
        [Authorize]
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetOrdersByUserAsync(Guid userId, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var response = await _orderService.GetOrdersByUserAsync(userId, page, pageSize);
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }
        /// <summary>
        /// Adds a product to an order.
        /// </summary>
        /// <param name="orderId">The id of the order.</param>
        /// <param name="productOrder">The product to be added.</param>
        /// <returns>No content.</returns>
        /// <response code="200">If the product is successfully added to the order.</response>
        /// <response code="400">If the product order is null, or if the product or order is not found, or if unable to add the product to the order.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /Order/{orderId}/products
        ///     {
        ///        "productId": "{productId}",
        ///        "quantity": 1
        ///     }
        /// </remarks>
        [Authorize]
        [HttpPost("{orderId}/products")]
        public async Task<IActionResult> AddProductToOrder(Guid orderId, [FromBody] ProductOrderRequest productOrder)
        {
            try
            {
                if (productOrder == null)
                {
                    return BadRequest(new { Message = "ProductOrder should not be null" });
                }

                var result = await _orderService.AddProductToOrderAsync(orderId, productOrder.ProductId, productOrder.Quantity);

                if (result)
                    return Ok();

                return BadRequest(new { Message = "Unable to add product to order" });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOrderStatusException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Updates the quantity of a product in an order.
        /// </summary>
        /// <param name="orderId">The id of the order.</param>
        /// <param name="productId">The id of the product.</param>
        /// <param name="newQuantity">The new quantity.</param>
        /// <returns>No content.</returns>
        /// <response code="200">If the product quantity is successfully updated.</response>
        /// <response code="400">If the new quantity is not greater than zero, or if unable to update the product quantity in the order, or if the order is not open.</response>
        /// <response code="404">If the product or order is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /Order/{orderId}/products/{productId}/quantity/{newQuantity}
        /// </remarks>
        [Authorize]
        [HttpPut("{orderId}/products/{productId}/quantity/{newQuantity}")]
        public async Task<IActionResult> UpdateProductQuantityInOrder(Guid orderId, Guid productId, int newQuantity)
        {
            try
            {
                var result = await _orderService.UpdateProductQuantityInOrderAsync(orderId, productId, newQuantity);

                if (result)
                    return Ok();

                return BadRequest(new { Message = "Unable to update product quantity in order" });
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOrderStatusException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }

        /// <summary>
        /// Removes a product from an order.
        /// </summary>
        /// <param name="orderId">The id of the order.</param>
        /// <param name="productId">The id of the product.</param>
        /// <returns>No content.</returns>
        /// <response code="200">If the product is successfully removed from the order.</response>
        /// <response code="400">If the order is not open, or if unable to remove the product from the order.</response>
        /// <response code="404">If the product or the order is not found.</response>
        /// <response code="500">If there is an internal server error.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     DELETE /Order/{orderId}/products/{productId}
        /// </remarks>
        [Authorize]
        [HttpDelete("{orderId}/products/{productId}")]
        public async Task<IActionResult> RemoveProductFromOrder(Guid orderId, Guid productId)
        {
            try
            {
                var result = await _orderService.RemoveProductFromOrderAsync(orderId, productId);

                if (result)
                    return Ok();

                return BadRequest(new { Message = "Unable to remove product from order" });
            }
            catch (ProductNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (OrderNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (InvalidOrderStatusException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "An error occurred while processing your request." });
            }
        }


    }
}
