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
                t.Id, -- Ajustado alias para bater com a propriedade 'Id' da classe pai
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

                -- Linha de corte (splitOn): Daqui para baixo mapeia a classe DetalheTicket
                tc.Id as IdComentario,
                tc.TicketId as IdTicket,
                tc.UserId as IdUsuario,
                tc.Content as Conteudo,
                tc.CreatedAt as DataCriacao -- O Dapper mapeará corretamente aqui por causa do splitOn
            FROM Tickets as t
                LEFT JOIN TicketComments tc ON tc.TicketId = t.Id
                INNER JOIN Users as u on u.Id = t.RequesterId and u.Role = 'User'
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

            var ticketDic = new Dictionary<int, ConsultaDetalheTicketResponse>();

            var resultado = await _db.QueryAsync<ConsultaDetalheTicketResponse, DetalheTicket, ConsultaDetalheTicketResponse>(query.ToString(),
            (ticket, detalhe) =>
            {
                if (!ticketDic.TryGetValue(ticket.Id, out var ticketAtual))
                {
                    ticketAtual = ticket;
                    ticketAtual.DetalhesTicket = new List<DetalheTicket>();
                    ticketDic.Add(ticketAtual.Id, ticketAtual);
                }

                if (detalhe != null && detalhe.IdComentario != 0)
                {
                    ticketAtual.DetalhesTicket?.Add(detalhe);
                }

                return ticketAtual;
            },
            splitOn: "IdComentario");

            return ticketDic.Values.ToList();
        }
    }


}