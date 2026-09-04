namespace TicketAPI.Interfaces
{
    public interface IUserRepository
    {
        Task<bool> UserExists(int userId);
    }
}