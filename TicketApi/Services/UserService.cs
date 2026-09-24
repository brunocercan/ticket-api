using TicketAPI.CustomExceptions;
using TicketAPI.Interfaces;

namespace TicketAPI.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;

        public async Task<List<ConsultaUsuarioResponse>> GetListaUsuariosAsync()
        {
            var response = new List<ConsultaUsuarioResponse>();
            var resultDto = await _userRepository.GetUserListAsync();

            resultDto.ForEach(u => response.Add(new ConsultaUsuarioResponse()
            {
                Nome = u.Name,
                DataCriacao = u.CreatedAt,
                EmailUsuario = u.Email,
                Funcao = u.Role,
                IdUsuario = u.Id
            }));

            return response;
        }
        public async Task<ConsultaUsuarioResponse> GetUsuarioPorIdAsync(int userId)
        {
            if (!await _userRepository.UserExists(userId))
            {
                throw new NotFoundException($"Usuario ID {userId}");
            }

            var userDto = await _userRepository.GetUserByIdAsync(userId);
            
            return new ConsultaUsuarioResponse()
            {
                DataCriacao = userDto.CreatedAt,
                EmailUsuario = userDto.Email,
                Funcao = userDto.Role,
                IdUsuario = userDto.Id,
                Nome = userDto.Name
            };
        }
    }
}