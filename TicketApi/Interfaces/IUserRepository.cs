using TicketAPI.DataTransferObjects;

namespace TicketAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserExists(int userId);
        Task<UsersDto> GetUserByIdAsync(int userId);
        Task<List<UsersDto>> GetUserListAsync();
    }
}