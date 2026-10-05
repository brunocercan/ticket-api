using TicketAPI.Models.Users;
using TicketAPI.Helpers;

namespace TicketAPI.Interfaces
{
    public interface IUserService
    {
        Task<PagedResponse<ConsultaUsuarioResponse>> GetListaUsuariosAsync(int pageNumber = 1, int pageSize = 10);
        Task<ConsultaUsuarioResponse> GetUsuarioPorIdAsync(int userId);
        Task<ConsultaUsuarioResponse> CreateUserAsync(CadastraUsuarioRequest request);
        Task<ConsultaUsuarioResponse> UpdateUserAsync(int id, AtualizaUsuarioRequest request);
        Task DeleteUserAsync(int id);
    }
}