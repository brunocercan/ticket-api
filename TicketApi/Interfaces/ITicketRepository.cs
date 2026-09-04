using TicketAPI.DataTransferObjects;
using TicketAPI.Models.Tickets;

namespace TicketAPI.Interfaces;

public interface ITicketRepository
{
    Task<List<TicketsDto>> GetTickets(ConsultaTicketsRequest ticketsRequest);
}