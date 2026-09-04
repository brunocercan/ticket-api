namespace TicketAPI.Data.EntityFramework;

using System.Threading.Tasks;
using DataTransferObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Tickets;
using TicketAPI.Interfaces;
using static Helpers.PropertiesHelper;

public class TicketRepository(AppDbContext context) : ITicketRepository
{
    private readonly AppDbContext _context = context;

    public async Task<List<TicketsDto>> GetTickets(ConsultaTicketsRequest ticketsRequest)
    {
        //Já verifica se todas as propriedades são null para retornar a lista total
        if (AreAllPropertiesNull(ticketsRequest))
        {
            return await _context.Tickets.ToListAsync();
        }

        //Verifica todas as propriedades para aplicar o filtro
        var query = _context.Tickets.AsQueryable();

        if (ticketsRequest.Id.HasValue)
        {
            query = query.Where(t => t.Id == ticketsRequest.Id);
        }

        if (!ticketsRequest.Prioridade.IsNullOrEmpty())
        {
            query = query.Where(t => t.Priority == ticketsRequest.Prioridade);
        }

        if (!ticketsRequest.Status.IsNullOrEmpty())
        {
            query = query.Where(t => t.Status == ticketsRequest.Status);
        }

        if (!ticketsRequest.Titulo.IsNullOrEmpty())
        {
            query = query.Where(t => t.Title == ticketsRequest.Titulo);
        }

        return await query.ToListAsync();
    }


}