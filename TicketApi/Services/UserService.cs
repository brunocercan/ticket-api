using TicketAPI.Interfaces;

namespace TicketAPI.Services
{
    public class UserService(IUserRepository userRepository) : IUserService
    {
        private readonly IUserRepository _userRepository = userRepository;
    }
}