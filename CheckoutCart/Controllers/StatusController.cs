using TechShop.BLL.Interface;
using TechShop.Helpers.Security.Contants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace TechShop.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatusController : ControllerBase
    {
        private readonly IStatusService _statusService;

        public StatusController(IStatusService statusService)
        {
            _statusService = statusService;
        }

        /// <summary>
        /// Retrieves all statuses.
        /// </summary>
        /// <returns>The list of statuses.</returns>
        /// <response code="200">Returns the list of statuses.</response>
        /// <response code="500">Internal server error.</response>
        [Authorize(Roles = $"{Roles.Employer},{Roles.Admin}" )]
        [HttpGet]
        public async Task<IActionResult> GetAll() 
        {
            try
            {
                var statusList = await _statusService.GetAllStatusAsync();
                return Ok(statusList);
            }
            catch (Exception) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "An error occurred." });
            }
        }
    }
}
