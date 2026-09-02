using Microsoft.AspNetCore.Mvc;
using TicketAPI.BusinessLayer;
using TicketAPI.Data;
using TicketAPI.Models.Tickets;

namespace TicketAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TicketsBL _tickets;
        
        public TicketController(AppDbContext context)
        {
            _context = context;
            _tickets = new TicketsBL(_context);
        }

        [HttpGet]
        [Route("tickets")]
        public async Task<IActionResult> GetTickets([FromQuery] ConsultaTicketsRequest consultaTicketsRequest)
        {
            var result = await _tickets.GetTicketsAsync(consultaTicketsRequest);
            return StatusCode(200, result);
        }
    }
}
