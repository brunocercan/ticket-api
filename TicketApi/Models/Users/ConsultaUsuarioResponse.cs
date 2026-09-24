public class ConsultaUsuarioResponse
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = "";
    public string EmailUsuario { get; set; } = "";
    public string Funcao { get; set; } = "";
    public DateTime? DataCriacao { get; set; }
}