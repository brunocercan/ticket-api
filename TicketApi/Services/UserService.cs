using TicketAPI.CustomExceptions;
using TicketAPI.Interfaces;
using TicketAPI.Models.Users;
using TicketAPI.DataTransferObjects;
using TicketAPI.Helpers;
using FluentValidation;
using CustomValidationException = TicketAPI.CustomExceptions.ValidationException;

namespace TicketAPI.Services
{
    public class UserService(IUserRepository userRepository, 
        IValidator<CadastraUsuarioRequest> cadastraUsuarioValidator,
        IValidator<AtualizaUsuarioRequest> atualizaUsuarioValidator) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IValidator<CadastraUsuarioRequest> _cadastraUsuarioValidator = cadastraUsuarioValidator;
        private readonly IValidator<AtualizaUsuarioRequest> _atualizaUsuarioValidator = atualizaUsuarioValidator;

        public async Task<PagedResponse<ConsultaUsuarioResponse>> GetListaUsuariosAsync(int pageNumber = 1, int pageSize = 10)
        {
            var resultDto = await _userRepository.GetUserListAsync(pageNumber, pageSize);

            var response = resultDto.Select(u => new ConsultaUsuarioResponse()
            {
                Nome = u.Name,
                DataCriacao = u.CreatedAt,
                EmailUsuario = u.Email,
                Funcao = u.Role,
                IdUsuario = u.Id
            }).ToList();

            return new PagedResponse<ConsultaUsuarioResponse>
            {
                Data = response,
                CurrentPage = resultDto.CurrentPage,
                TotalPages = resultDto.TotalPages,
                PageSize = resultDto.PageSize,
                TotalCount = resultDto.TotalCount
            };
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

        public async Task<ConsultaUsuarioResponse> CreateUserAsync(CadastraUsuarioRequest request)
        {
            var validationResult = await _cadastraUsuarioValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new CustomValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            var userDto = new UsersDto
            {
                Name = request.Nome,
                Email = request.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha),
                Role = request.Funcao,
                CreatedAt = DateTime.Now
            };

            await _userRepository.CreateUserAsync(userDto);

            return new ConsultaUsuarioResponse
            {
                IdUsuario = userDto.Id,
                Nome = userDto.Name,
                EmailUsuario = userDto.Email,
                Funcao = userDto.Role,
                DataCriacao = userDto.CreatedAt
            };
        }

        public async Task<ConsultaUsuarioResponse> UpdateUserAsync(int id, AtualizaUsuarioRequest request)
        {
            var validationResult = await _atualizaUsuarioValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                throw new CustomValidationException(validationResult.Errors.Select(e => e.ErrorMessage));
            }

            if (!await _userRepository.UserExists(id))
            {
                throw new NotFoundException($"Usuario ID {id}");
            }

            var existingUser = await _userRepository.GetUserByIdAsync(id);

            existingUser.Name = request.Nome;
            existingUser.Email = request.Email;
            existingUser.Role = request.Funcao;
            if (!string.IsNullOrEmpty(request.Senha))
            {
                existingUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Senha);
            }

            await _userRepository.UpdateUserAsync(id, existingUser);

            return new ConsultaUsuarioResponse
            {
                IdUsuario = existingUser.Id,
                Nome = existingUser.Name,
                EmailUsuario = existingUser.Email,
                Funcao = existingUser.Role,
                DataCriacao = existingUser.CreatedAt
            };
        }

        public async Task DeleteUserAsync(int id)
        {
            if (!await _userRepository.UserExists(id))
            {
                throw new NotFoundException($"Usuario ID {id}");
            }

            await _userRepository.DeleteUserAsync(id);
        }
    }
}