using TicketAPI.DataTransferObjects;
using TicketAPI.Helpers;

namespace TicketAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserExists(int userId);
        Task<UsersDto> GetUserByIdAsync(int userId);
        Task<PagedList<UsersDto>> GetUserListAsync(int pageNumber, int pageSize);
        Task<UsersDto?> GetUserByEmailAsync(string email);
        Task CreateUserAsync(UsersDto user);
        Task UpdateUserAsync(int userId, UsersDto user);
        Task DeleteUserAsync(int userId);
    }
}