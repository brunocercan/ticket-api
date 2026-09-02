public class ConsultaTicketsResponse
{
    public int Id { get; set; }
    public string Titulo { get; set; } = "";
    public string Descricao { get; set; } = "";
    public string Prioridade { get; set; } = "";
    public string Status { get; set; } = "";
    public int IdCategoria { get; set; }
    public int IdSolicitante { get; set; }
    public int? IdVinculado { get; set; }
    public DateTime DataCriacao { get; set; }
    public DateTime? DataAtualizacao { get; set; }
    public DateTime? DataFechamento { get; set; }
}