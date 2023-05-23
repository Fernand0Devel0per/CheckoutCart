using TechShop.BBL.Interface;
using TechShop.Dtos.User;
using TechShop.Helpers.Exceptions;
using TechShop.Helpers.Security.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;
using System.Security.Claims;

namespace TechShop.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Fetches a user based on their username.
        /// </summary>
        /// <param name="username">The username of the user to fetch.</param>
        /// <returns>An action result containing the UserCreateResponse if the user is found, NotFound if the user is not found, and BadRequest if any other exception occurs.</returns>
        /// <response code="200">Returns the UserCreateResponse for the user.</response>
        /// <response code="404">If no user with the specified username is found.</response>
        /// <response code="400">If any other exception occurs.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     GET /username/{username}
        ///
        /// </remarks>
        [Authorize]
        [HttpGet("username/{username}")]
        public async Task<ActionResult<UserCreateResponse>> GetUser(string username)
        {
            try
            {
                var user = await _userService.GetUserByUsernameAsync(username);
                return Ok(user);
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="userDto">The UserCreateRequest object that contains the details of the user to create. It includes Username, Password, ConfirmPassword, and Role.</param>
        /// <returns>A status code 201 (Created) with the UserCreateResponse for the created user if successful, BadRequest if an ArgumentException occurs or if any other exception occurs.</returns>
        /// <response code="201">Returns the UserCreateResponse for the created user.</response>
        /// <response code="400">If an ArgumentException occurs or if any other exception occurs.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /
        ///     {
        ///         "username": "test",
        ///         "password": "password123",
        ///         "confirmPassword": "password123",
        ///         "role": "Admin"
        ///     }
        ///
        /// </remarks>
        [Authorize(Roles = Roles.Admin)]
        [HttpPost]
        public async Task<ActionResult<UserCreateResponse>> CreateUser([FromBody] UserCreateRequest userDto)
        {
            try
            {
                var user = await _userService.CreateUserAsync(userDto);
                return StatusCode(StatusCodes.Status201Created, user);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "An unexpected error occurred." });
            }
        }

        /// <summary>
        /// Updates an existing user.
        /// </summary>
        /// <param name="userDto">The UserUpdateRequest object that contains the details of the user to update. It includes Password, NewPassword, ConfirmNewPassword, and Role.</param>
        /// <returns>NoContent if the update is successful, NotFound if the user is not found, Unauthorized if an AuthenticationException occurs, BadRequest if an ArgumentException occurs, or InternalServerError for any other exceptions.</returns>
        /// <response code="204">If the user was updated successfully.</response>
        /// <response code="400">If an ArgumentException occurs.</response>
        /// <response code="401">If an AuthenticationException occurs.</response>
        /// <response code="404">If the user is not found.</response>
        /// <response code="500">If any other exception occurs.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     PUT /
        ///     {
        ///         "password": "oldPassword123",
        ///         "newPassword": "newPassword123",
        ///         "confirmNewPassword": "newPassword123",
        ///         "role": "Admin"
        ///     }
        ///
        /// </remarks>
        [Authorize(Roles = Roles.Admin)]
        [HttpPut]
        public async Task<IActionResult> UpdateUser( [FromBody] UserUpdateRequest userDto)
        {

            var usernameClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            try
            {
                var isSuccess = await _userService.UpdateUserAsync(userDto, usernameClaim.Value); 
                if (isSuccess)
                {
                    return NoContent();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (UserNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (AuthenticationException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
              
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred." });
            }
        }
    }
}
