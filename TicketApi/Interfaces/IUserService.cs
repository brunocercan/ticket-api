namespace TicketAPI.Interfaces
{
    public interface IUserService
    {
        Task<List<ConsultaUsuarioResponse>> GetListaUsuariosAsync();
        Task<ConsultaUsuarioResponse> GetUsuarioPorIdAsync(int userId);
    }
}