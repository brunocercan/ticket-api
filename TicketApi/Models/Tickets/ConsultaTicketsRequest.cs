using TicketAPI.Helpers;

namespace TicketAPI.Models.Tickets;

public class ConsultaTicketsRequest : PaginationParams
{
    // Request para filtros simples de consulta dos tickets, todos são opcionais para busca, caso nenhum preenchido é listado todos
    public int? Id { get; set; }
    public string? Titulo { get; set; }
    public string? Prioridade { get; set; }
    public string? Status { get; set; }
    public int? IdSolicitante { get; set; }
}