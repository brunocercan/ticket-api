using TicketAPI.Models.TicketComments;
using TicketAPI.Models.Tickets;
using TicketAPI.Helpers;

namespace TicketAPI.Interfaces;
public interface ITicketService
{
    Task<PagedResponse<ConsultaTicketsResponse>> GetTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest);
    Task<List<ConsultaDetalheTicketResponse>> GetDetailTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest);
    Task PostNewTicketComment(CadastraComentarioTicket comentarioRequest);
    Task PostNewTicket(CadastraTicketRequest cadastraTicketRequest);
    Task DeleteTicket(int id);
    Task AtualizaTicket(AtualizaTicketRequest request);
    Task<ConsultaTicketsResponse> GetSingleTicketAsync(int id);
}