using Microsoft.AspNetCore.Mvc;
using TicketAPI.Interfaces;
using TicketAPI.Models.TicketComments;
using TicketAPI.Models.Tickets;

namespace TicketAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TicketController(ITicketService tickets) : ControllerBase
    {
        private readonly ITicketService _tickets = tickets;

        /// <summary>
        /// Endpoint para consulta rápida dos tickets utilizando o EntityFramework
        /// </summary>
        /// <param name="consultaTicketsRequest"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(typeof(ConsultaTicketsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTickets([FromQuery] ConsultaTicketsRequest consultaTicketsRequest)
        {
            var result = await _tickets.GetTicketsAsync(consultaTicketsRequest);
            return Ok(result);
        }

        /// <summary>
        /// Endpoint para consulta detalhada do ticket através de Query utilizando o Dapper
        /// </summary>
        /// <param name="consultaTicketsRequest"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("details")]
        [ProducesResponseType(typeof(ConsultaDetalheTicketResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTicketsDetails([FromQuery] ConsultaTicketsRequest consultaTicketsRequest)
        {
            var result = await _tickets.GetDetailTicketsAsync(consultaTicketsRequest);
            return Ok(result);
        }

        [HttpPost]
        [Route("comment")]
        [ProducesResponseType(typeof(void), StatusCodes.Status201Created)]
        public async Task<IActionResult> PostNewTicketComment([FromBody] CadastraComentarioTicket comentarioRequest)
        {
            await _tickets.PostNewTicketComment(comentarioRequest);
            return StatusCode(201, "Comentario do ticket cadastrado com sucesso!");
        }
    }
}
