using TicketAPI.DataTransferObjects;
using TicketAPI.Models.Tickets;
using TicketAPI.Helpers;

namespace TicketAPI.Interfaces;

public interface ITicketRepository
{
    Task<PagedList<TicketsDto>> GetTicketsAsync(ConsultaTicketsRequest ticketsRequest);
    Task<bool> TicketExists(int ticketId);
    Task DeleteTicketsAsync(int ticketId);
    Task CreateTicketAsync(TicketsDto request);
    Task UpdateTicketsAsync(int ticketId, TicketsDto ticket);
    Task<TicketsDto> GetSingleTicketAsync(int ticketId);
}