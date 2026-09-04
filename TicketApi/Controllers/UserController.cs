using Microsoft.AspNetCore.Mvc;
using TicketAPI.Interfaces;

namespace TicketAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;
        [HttpGet]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            return Ok("User Get!");
        }
    }
}
