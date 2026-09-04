using Microsoft.AspNetCore.Mvc;
using TicketAPI.Interfaces;
using TicketAPI.Models.Tickets;

namespace TicketAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly ITicketService _tickets;

        public TicketController(ITicketService tickets)
        {
            _tickets = tickets;
        }
        
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
            var result = await _tickets.GetDetalheTicketsAsync(consultaTicketsRequest);
            return Ok(result);
        }
    }
}
