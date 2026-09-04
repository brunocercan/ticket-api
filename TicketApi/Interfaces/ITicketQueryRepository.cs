using TicketAPI.Models.Tickets;

namespace TicketAPI.Interfaces
{
    public interface ITicketQueryRepository
    {
        Task<List<ConsultaDetalheTicketResponse>> GetConsultaDetalheTicketResponsesAsync(ConsultaTicketsRequest request);
    }
}