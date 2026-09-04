using TicketAPI.CustomExceptions;
using Microsoft.IdentityModel.Tokens;
using TicketAPI.Models.Tickets;
using TicketAPI.Interfaces;

namespace TicketAPI.Services
{
    public class TicketService(ITicketRepository ticketRepository, ITicketQueryRepository ticketQueryRepository) : ITicketService
    {
        private readonly ITicketRepository _ticketRepository = ticketRepository;
        private readonly ITicketQueryRepository _ticketQueryRepository = ticketQueryRepository;

        public async Task<List<ConsultaTicketsResponse>> GetTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest)
        {
            var resultData = await _ticketRepository.GetTickets(consultaTicketsRequest);
            if (resultData.IsNullOrEmpty())
            {
                throw new NotFoundException();
            }

            return resultData.Select(r => new ConsultaTicketsResponse()
            {
                    Id = r.Id,
                    Titulo = r.Title,
                    Descricao = r.Description,
                    Prioridade = r.Priority,
                    Status = r.Status,
                    IdCategoria = r.CategoryId,
                    IdVinculado = r.AssignedToId,
                    IdSolicitante = r.RequesterId,
                    DataCriacao = r.CreatedAt,
                    DataAtualizacao = r.UpdatedAt,
                    DataFechamento = r.ClosedAt
            }).ToList();
        }

        public async Task<List<ConsultaDetalheTicketResponse>> GetDetalheTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest)
        {
            return await _ticketQueryRepository.GetConsultaDetalheTicketResponsesAsync(consultaTicketsRequest) ?? throw new NotFoundException();
        }
    }

}
