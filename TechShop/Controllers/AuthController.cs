using TechShop.BBL.Interface;
using TechShop.Dtos.User;
using Microsoft.AspNetCore.Mvc;
using System.Security.Authentication;

namespace TechShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        /// <summary>
        /// Authenticates a user.
        /// </summary>
        /// <param name="loginRequest">The login request containing the user's credentials.</param>
        /// <returns>A JWT token if authentication is successful, Unauthorized otherwise.</returns>
        /// <response code="200">Returns the JWT token.</response>
        /// <response code="401">If authentication fails.</response>
        /// <remarks>
        /// Sample request:
        ///
        ///     POST /login
        ///     {
        ///        "username": "johndoe",
        ///        "password": "password123"
        ///     }
        ///
        /// </remarks>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                var token = await _authenticationService.AuthenticateAsync(loginRequest);
                if (token == null)
                {
                    return Unauthorized();
                }
                return Ok(token);
            }
            catch (AuthenticationException)
            {
                return Unauthorized();
            }
        }
    }
}
