using TicketAPI.DataTransferObjects;
using TicketAPI.Models.Tickets;

namespace TicketAPI.Interfaces;

public interface ITicketRepository
{
    Task<List<TicketsDto>> GetTicketsAsync(ConsultaTicketsRequest ticketsRequest);
    Task<bool> TicketExists(int ticketId);
    Task UpdateTicketsAsync(int ticketId);
    Task DeleteTicketsAsync(int ticketId);
    Task CreateTicketAsync(CadastraTicketRequest request);
}