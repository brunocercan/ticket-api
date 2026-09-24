using Microsoft.AspNetCore.Mvc;
using TicketAPI.Interfaces;

namespace TicketAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        /// <summary>
        /// Endpoint para consultar todos usuários
        /// </summary>
        private readonly IUserService _userService = userService;
        [HttpGet]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _userService.GetListaUsuariosAsync();
            return Ok(response);
        }

        /// <summary>
        /// Endpoint de consulta usuario por ID
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromRoute] int id)
        {
            var response = await _userService.GetUsuarioPorIdAsync(id);
            return Ok(response);
        }
    }
}
