using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using TicketAPI.Interfaces;
using TicketAPI.Models.TicketComments;
using TicketAPI.Models.Tickets;
using TicketAPI.Helpers;

namespace TicketAPI.Controllers
{
    [Authorize]
    [Route("api/tickets")]
    [ApiController]
    public class TicketController(ITicketService tickets) : ControllerBase
    {
        private readonly ITicketService _tickets = tickets;

        private int GetCurrentUserId() => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
        private string GetCurrentUserRole() => User.FindFirst(ClaimTypes.Role)?.Value ?? "";

        /// <summary>
        /// Endpoint para consulta rápida dos tickets com paginação
        /// User: apenas seus tickets | Support/Admin: todos os tickets
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<ConsultaTicketsResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTickets([FromQuery] ConsultaTicketsRequest consultaTicketsRequest)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();
            
            // User só vê seus próprios tickets
            if (userRole == "User")
            {
                consultaTicketsRequest.IdSolicitante = userId;
            }

            var result = await _tickets.GetTicketsAsync(consultaTicketsRequest);
            return Ok(result);
        }

        /// <summary>
        /// Busca único ticket
        /// User: apenas se for solicitante | Support/Admin: qualquer ticket
        /// </summary>
        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(typeof(ConsultaTicketsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSingleTicket([FromRoute] int id)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            var ticket = await _tickets.GetSingleTicketAsync(id);
            
            // Verifica permissão
            if (userRole == "User" && ticket.IdSolicitante != userId)
            {
                return Forbid();
            }

            return Ok(ticket);
        }

        /// <summary>
        /// Endpoint para consulta detalhada do ticket através de Query utilizando o Dapper
        /// User: apenas seus tickets | Support/Admin: todos os tickets
        /// </summary>
        [HttpGet]
        [Route("details")]
        [ProducesResponseType(typeof(ConsultaDetalheTicketResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTicketsDetails([FromQuery] ConsultaTicketsRequest consultaTicketsRequest)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            if (userRole == "User")
            {
                consultaTicketsRequest.IdSolicitante = userId;
            }

            var result = await _tickets.GetDetailTicketsAsync(consultaTicketsRequest);
            return Ok(result);
        }

        /// <summary>
        /// Endpoint para adicionar comentário a ticket existente
        /// User: pode comentar nos seus tickets | Support/Admin: pode comentar em qualquer ticket
        /// </summary>
        [HttpPost]
        [Route("comment")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> PostNewTicketComment([FromBody] CadastraComentarioTicket comentarioRequest)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            // Verifica se pode comentar no ticket
            var ticket = await _tickets.GetSingleTicketAsync(comentarioRequest.TicketId);
            if (userRole == "User" && ticket.IdSolicitante != userId)
            {
                return Forbid();
            }

            // Define o usuário do comentário como o usuário logado
            comentarioRequest.UserId = userId;

            await _tickets.PostNewTicketComment(comentarioRequest);
            return StatusCode(201, "Comentario do ticket cadastrado com sucesso!");
        }

        /// <summary>
        /// Endpoint para criar novo ticket
        /// User: cria ticket como solicitante | Support/Admin: pode criar para qualquer solicitante
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostNewTicket([FromBody] CadastraTicketRequest cadastraTicketRequest)
        {
            var userId = GetCurrentUserId();
            var userRole = GetCurrentUserRole();

            // User só pode criar tickets para si mesmo
            if (userRole == "User")
            {
                cadastraTicketRequest.IdSolicitante = userId;
            }

            await _tickets.PostNewTicket(cadastraTicketRequest);
            return StatusCode(201, "Ticket cadastrado com sucesso!");
        }

        /// <summary>
        /// Endpoint para deletar ticket
        /// Apenas Admin e Support podem deletar
        /// </summary>
        [HttpDelete]
        [Authorize(Roles = "Admin,Support")]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DeleteTicket([FromRoute] int id)
        {
            await _tickets.DeleteTicket(id);
            return NoContent();
        }

        /// <summary>
        /// Endpoint para atualizar ticket
        /// User: NÃO pode editar | Support/Admin: pode editar
        /// </summary>
        [HttpPut]
        [Authorize(Roles = "Admin,Support")]
        [Route("{id}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateTicket([FromRoute] int id, [FromBody] AtualizaTicketRequest request)
        {
            if (id != request.Id)
            {
                return BadRequest();
            }

            await _tickets.AtualizaTicket(request);
            return NoContent();
        }
        
    }
}
