using CustomExceptions;
using Data;
using Data.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Tickets;

namespace BusinessLayer
{
    public class TicketsBL(AppDbContext context)
    {
        private readonly TicketsData _ticketsdata = new TicketsData(context);

        public async Task<List<ConsultaTicketsResponse>> GetTicketsAsync(ConsultaTicketsRequest consultaTicketsRequest)
        {
            var listReponse = new List<ConsultaTicketsResponse>();
            var response = new ConsultaTicketsResponse();

            var resultData = await _ticketsdata.GetTickets(consultaTicketsRequest);

            if (resultData.IsNullOrEmpty())
            {
                throw new NotFoundException();
            }

            resultData.ForEach(r =>
                {
                    response = new ConsultaTicketsResponse()
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
                    };
                    listReponse.Add(response);
                }
            );

            return listReponse;
        }
    }

}
