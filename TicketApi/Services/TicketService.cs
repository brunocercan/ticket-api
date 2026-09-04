using TicketAPI.CustomExceptions;
using TicketAPI.Models.Tickets;
using TicketAPI.Interfaces;
using TicketAPI.Models.TicketComments;
using TicketAPI.DataTransferObjects;

namespace TicketAPI.Services
{
    public class TicketService(ITicketRepository ticketRepository, 
        ITicketQueryRepository ticketQueryRepository, 
        ITicketCommentRepository ticketCommentRepository,
        IUserRepository userRepository) : ITicketService
    {
        private readonly ITicketRepository _ticketRepository = ticketRepository;
        private readonly ITicketQueryRepository _ticketQueryRepository = ticketQueryRepository;
        private readonly ITicketCommentRepository _ticketCommentRepository = ticketCommentRepository;
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<List<ConsultaTicketsResponse>> GetTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest)
        {
            var resultData = await _ticketRepository.GetTicketsAsync(consultaTicketsRequest);
            
            if (resultData.Count == 0)
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
        public async Task<List<ConsultaDetalheTicketResponse>> GetDetailTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest)
        {
            return await _ticketQueryRepository.GetConsultaDetalheTicketResponsesAsync(consultaTicketsRequest) ?? throw new NotFoundException();
        }
        public async Task PostNewTicketComment(CadastraComentarioTicket comentarioRequest)
        {
            if(!await _ticketRepository.TicketExists(comentarioRequest.TicketId))
            {
                throw new NotFoundException($"Ticket Id {comentarioRequest.TicketId}");
            }

            if (!await _userRepository.UserExists(comentarioRequest.UserId))
            {
                throw new NotFoundException($"User Id {comentarioRequest.UserId}");
            }

            var ticketCommentDto = new TicketCommentsDto()
            {
                Content = comentarioRequest.Content,
                CreatedAt = comentarioRequest.CreatedAt,
                TicketId = comentarioRequest.TicketId,
                UserId = comentarioRequest.UserId
            };

            await _ticketCommentRepository.CreateTicketCommentAsync(ticketCommentDto);
        }
    }

}
