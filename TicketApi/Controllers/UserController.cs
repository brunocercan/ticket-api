using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TicketAPI.Interfaces;
using TicketAPI.Models.Users;
using TicketAPI.Helpers;

namespace TicketAPI.Controllers
{
    [Authorize]
    [Route("api/users")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Endpoint para consultar todos usuários com paginação
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ConsultaUsuarioResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var response = await _userService.GetListaUsuariosAsync(pageNumber, pageSize);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint de consulta usuario por ID
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ConsultaUsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserById([FromRoute] int id)
        {
            var response = await _userService.GetUsuarioPorIdAsync(id);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint para criar um novo usuário (apenas Admin)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(ConsultaUsuarioResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateUser([FromBody] CadastraUsuarioRequest request)
        {
            var response = await _userService.CreateUserAsync(request);
            return StatusCode(201, response);
        }

        /// <summary>
        /// Endpoint para atualizar um usuário (apenas Admin)
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [ProducesResponseType(typeof(ConsultaUsuarioResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateUser([FromRoute] int id, [FromBody] AtualizaUsuarioRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest("ID da rota não corresponde ao ID do corpo da requisição.");
            }

            var response = await _userService.UpdateUserAsync(id, request);
            return Ok(response);
        }

        /// <summary>
        /// Endpoint para deletar um usuário (apenas Admin)
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "Admin")]
        [Route("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteUser([FromRoute] int id)
        {
            await _userService.DeleteUserAsync(id);
            return NoContent();
        }
    }
}
