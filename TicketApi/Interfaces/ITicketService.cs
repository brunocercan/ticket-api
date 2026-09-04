using TicketAPI.Models.Tickets;

namespace TicketAPI.Interfaces;
public interface ITicketService
{
    Task<List<ConsultaTicketsResponse>> GetTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest);
    Task<List<ConsultaDetalheTicketResponse>> GetDetalheTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest);
}