
using TicketAPI.DataTransferObjects;

namespace TicketAPI.Interfaces
{
    public interface ITicketCommentRepository
    {
        Task CreateTicketCommentAsync(TicketCommentsDto ticketCommentsDto);
    }
}