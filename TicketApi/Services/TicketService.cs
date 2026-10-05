using TicketAPI.CustomExceptions;
using TicketAPI.Models.Tickets;
using TicketAPI.Interfaces;
using TicketAPI.Models.TicketComments;
using TicketAPI.DataTransferObjects;
using TicketAPI.Helpers;
using FluentValidation;
using CustomValidationException = TicketAPI.CustomExceptions.ValidationException;

namespace TicketAPI.Services
{
    public class TicketService(ITicketRepository ticketRepository, 
        ITicketQueryRepository ticketQueryRepository, 
        ITicketCommentRepository ticketCommentRepository,
        IUserRepository userRepository,
        IValidator<CadastraTicketRequest> cadastraTicketValidator,
        IValidator<AtualizaTicketRequest> atualizaTicketValidator,
        IValidator<CadastraComentarioTicket> cadastraComentarioValidator) : ITicketService
    {
        private readonly ITicketRepository _ticketRepository = ticketRepository;
        private readonly ITicketQueryRepository _ticketQueryRepository = ticketQueryRepository;
        private readonly ITicketCommentRepository _ticketCommentRepository = ticketCommentRepository;
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IValidator<CadastraTicketRequest> _cadastraTicketValidator = cadastraTicketValidator;
        private readonly IValidator<AtualizaTicketRequest> _atualizaTicketValidator = atualizaTicketValidator;
        private readonly IValidator<CadastraComentarioTicket> _cadastraComentarioValidator = cadastraComentarioValidator;

        public async Task<PagedResponse<ConsultaTicketsResponse>> GetTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest)
        {
            var resultData = await _ticketRepository.GetTicketsAsync(consultaTicketsRequest);

            var response = resultData.Select(r => new ConsultaTicketsResponse()
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

            return new PagedResponse<ConsultaTicketsResponse>
            {
                Data = response,
                CurrentPage = resultData.CurrentPage,
                TotalPages = resultData.TotalPages,
                PageSize = resultData.PageSize,
                TotalCount = resultData.TotalCount
            };
        }

        public async Task<List<ConsultaDetalheTicketResponse>> GetDetailTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest)
        {
            return await _ticketQueryRepository.GetConsultaDetalheTicketResponsesAsync(consultaTicketsRequest) ?? throw new NotFoundException();
        }

        public async Task PostNewTicketComment(CadastraComentarioTicket comentarioRequest)
        {
            var validationResult = await _cadastraComentarioValidator.ValidateAsync(comentarioRequest);
            if (!validationResult.IsValid)
            {
                throw new CustomValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
            }

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
                CreatedAt = DateTime.Now,
                TicketId = comentarioRequest.TicketId,
                UserId = comentarioRequest.UserId
            };

            await _ticketCommentRepository.CreateTicketCommentAsync(ticketCommentDto);
        } 

        public async Task PostNewTicket(CadastraTicketRequest cadastraTicketRequest)
        {
            var validationResult = await _cadastraTicketValidator.ValidateAsync(cadastraTicketRequest);
            if (!validationResult.IsValid)
            {
                throw new CustomValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var ticketDto = new TicketsDto()
            {
                CreatedAt = DateTime.Now,
                AssignedToId = cadastraTicketRequest.IdVinculado,
                CategoryId = cadastraTicketRequest.IdCategoria,
                ClosedAt = null,
                UpdatedAt = null,
                Description = cadastraTicketRequest.Descricao,
                Priority = cadastraTicketRequest.Prioridade,
                RequesterId = cadastraTicketRequest.IdSolicitante,
                Status = cadastraTicketRequest.Status,
                Title = cadastraTicketRequest.Titulo
            };

            await _ticketRepository.CreateTicketAsync(ticketDto);
        }

        public async Task DeleteTicket(int id)
        {
            if (!await _ticketRepository.TicketExists(id))
            {
                throw new NotFoundException($"Ticket Id {id}");
            }

            await _ticketRepository.DeleteTicketsAsync(id);
        }

        public async Task AtualizaTicket(AtualizaTicketRequest request)
        {
            var validationResult = await _atualizaTicketValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new CustomValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            if (!await _ticketRepository.TicketExists(request.Id))
            {
                throw new NotFoundException($"Ticket Id {request.Id}");
            }

            var ticket = new TicketsDto()
            {
                Id = request.Id,
                AssignedToId = request.IdVinculado,
                CategoryId = request.IdCategoria,
                ClosedAt = request.DataFechamento,
                UpdatedAt = request.DataAlteracao,
                CreatedAt = request.DataCriacao,
                Description = request.Descricao,
                Priority = request.Prioridade,
                RequesterId = request.IdSolicitante,
                Status = request.Status,
                Title = request.Titulo
            };

            await _ticketRepository.UpdateTicketsAsync(request.Id, ticket);
        }

        public async Task<ConsultaTicketsResponse> GetSingleTicketAsync(int id)
        {
            if (!await _ticketRepository.TicketExists(id))
            {
                throw new NotFoundException($"Ticket Id {id}");
            }

            var ticket = await _ticketRepository.GetSingleTicketAsync(id);

            return new ConsultaTicketsResponse()
            {
                DataAtualizacao = ticket.UpdatedAt,
                DataCriacao = ticket.CreatedAt,
                DataFechamento = ticket.ClosedAt,
                Descricao = ticket.Description,
                Id = ticket.Id,
                IdCategoria = ticket.CategoryId,
                IdSolicitante = ticket.RequesterId,
                IdVinculado = ticket.AssignedToId,
                Prioridade = ticket.Priority,
                Status = ticket.Status,
                Titulo = ticket.Title
            };
        }
    }

}
