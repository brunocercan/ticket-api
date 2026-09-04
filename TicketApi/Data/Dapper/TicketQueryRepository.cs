using System.Data;
using System.Text;
using Dapper;
using Microsoft.IdentityModel.Tokens;
using TicketAPI.Interfaces;
using TicketAPI.Models.Tickets;

namespace TicketAPI.Data.Dapper
{
    public class TicketQueryRepository(IDbConnection db) : ITicketQueryRepository
    {
        private readonly IDbConnection _db = db;

        public async Task<List<ConsultaDetalheTicketResponse>> GetConsultaDetalheTicketResponsesAsync(ConsultaTicketsRequest request)
        {
            var parameters = new DynamicParameters();
            var query = new StringBuilder(@"SELECT
                t.Title as Titulo,
                t.Description as Descricao,
                t.Priority as Prioridade,
                t.Status,
                t.CategoryId as IdCategoria,
                c.Name as NomeCategoria,
                t.RequesterId as IdSolicitante,
                u.Name as NomeSolicitante,
                t.AssignedToId as IdVinculado,
                r.Name as NomeResponsavelChamado,
                t.CreatedAt as DataCriacao,
                t.UpdatedAt as DataAtualizacao,
                t.ClosedAt as DataFechamento,
                tc.Content as DetalheTicket
            FROM Tickets as t
                LEFT JOIN TicketComments tc ON tc.TicketId = t.Id
                --Buscando Usuario
                INNER JOIN Users as u on u.Id = t.RequesterId and u.Role = 'User'
                --Buscando Responsavel pelo chamado 
                LEFT JOIN Users as r on r.Id = t.AssignedToId and r.Role = 'Support'
                INNER JOIN Categories as c on c.Id = t.CategoryId
                WHERE 1=1");

            if (request.Id.HasValue)
            {
                parameters.Add("Id", request.Id);
                query.AppendLine("AND t.Id = @id");
            }

            if (!request.Prioridade.IsNullOrEmpty())
            {
                parameters.Add("Priority", request.Prioridade);
                query.AppendLine("AND t.Priority = @Priority");
            }

            if (!request.Status.IsNullOrEmpty())
            {
                parameters.Add("Status", request.Status);
                query.AppendLine("AND t.Status = @Status");
            }

            if (!request.Titulo.IsNullOrEmpty())
            {
                parameters.Add("Title", request.Titulo);
                query.AppendLine("AND t.Title = @Title");
            }

            var result = await _db.QueryAsync<ConsultaDetalheTicketResponse>(sql: query.ToString(), param: parameters);
            return result.ToList();
        }
    }


}