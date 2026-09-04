using TicketAPI.Models.TicketComments;
using TicketAPI.Models.Tickets;

namespace TicketAPI.Interfaces;
public interface ITicketService
{
    Task<List<ConsultaTicketsResponse>> GetTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest);
    Task<List<ConsultaDetalheTicketResponse>> GetDetailTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest);
    Task PostNewTicketComment(CadastraComentarioTicket comentarioRequest);
}