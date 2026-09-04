using Microsoft.EntityFrameworkCore;
using TicketAPI.DataTransferObjects;
using TicketAPI.Interfaces;

namespace TicketAPI.Data.EntityFramework
{
    public class TicketCommentRepository(AppDbContext context) : ITicketCommentRepository
    {
        private readonly AppDbContext _context = context;

        public async Task CreateTicketCommentAsync(TicketCommentsDto ticketCommentsDto)
        {
            await _context.TicketComments.AddAsync(ticketCommentsDto);
            await _context.SaveChangesAsync();
        }
    }
}