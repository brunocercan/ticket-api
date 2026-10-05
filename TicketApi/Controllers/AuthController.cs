using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using TicketAPI.Models.Auth;
using TicketAPI.Models.Users;
using TicketAPI.Services;

namespace TicketAPI.Controllers
{
    [AllowAnonymous]
    [Route("api/auth")]
    [ApiController]
    public class AuthController(AuthService authService) : ControllerBase
    {
        private readonly AuthService _authService = authService;

        /// <summary>
        /// Endpoint para login
        /// </summary>
        [HttpPost("login")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var token = await _authService.AuthenticateAsync(request.Email, request.Senha);

            if (token == null)
            {
                return Unauthorized(new { message = "Email ou senha inválidos" });
            }

            return Ok(new { token });
        }

        /// <summary>
        /// Endpoint para registro de usuário
        /// </summary>
        [HttpPost("register")]
        [ProducesResponseType(typeof(ConsultaUsuarioResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] CadastraUsuarioRequest request)
        {
            var user = await _authService.RegisterAsync(request);

            if (user == null)
            {
                return BadRequest(new { message = "Email já cadastrado" });
            }

            return StatusCode(201, user);
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; } = "";
        public string Senha { get; set; } = "";
    }
}