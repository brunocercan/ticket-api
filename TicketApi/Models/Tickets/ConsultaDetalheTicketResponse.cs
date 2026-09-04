public class ConsultaDetalheTicketResponse
{
    //Model com mais detalhes do ticket utilizado para consulta no Dapper com Query mais complexa
    /// <summary>
    /// Id do ticket apresentado
    /// </summary>
    public int Id { get; set; }
    /// <summary>
    /// Titulo do ticket
    /// </summary>
    public string Titulo { get; set; } = "";
    /// <summary>
    /// Descrição do ticket
    /// </summary>
    public string Descricao { get; set; } = "";
    /// <summary>
    /// Prioridade do ticket [High, Critical, Medium, Low]
    /// </summary>
    public string Prioridade { get; set; } = "";
    /// <summary>
    /// Status do ticket [Open, InProgress, Resolved]
    /// </summary>
    public string Status { get; set; } = "";
    /// <summary>
    /// Id da categoria, utilizado internamente
    /// </summary>
    public int IdCategoria { get; set; }
    /// <summary>
    /// Categoria do ticket [Network, Software, Access, Hardware]
    /// </summary>
    public string NomeCategoria { get; set; } = "";
    /// <summary>
    /// Id do solicitante
    /// </summary>
    public int IdSolicitante { get; set; }
    /// <summary>
    /// Nome do solicitante, pego através do IdSolicitante
    /// </summary>
    public string NomeSolicitante { get; set; } = "";
    /// <summary>
    /// Id vinculado ao responsável verificando o ticket
    /// </summary>
    public int? IdVinculado { get; set; }
    /// <summary>
    /// Nome do responsavel a verificar o ticket
    /// </summary>
    public string NomeResponsavelChamado { get; set; } = "";
    /// <summary>
    /// Data da criação do ticket
    /// </summary>
    public DateTime DataCriacao { get; set; }
    /// <summary>
    /// Data da ultima atualização do ticket
    /// </summary>
    public DateTime? DataAtualizacao { get; set; }
    /// <summary>
    /// Data da finalização do ticket
    /// </summary>
    public DateTime? DataFechamento { get; set; }
    /// <summary>
    /// Descrição com maiores detalhes do ticket
    /// </summary>
    public string DetalheTicket { get; set; } = "";
}