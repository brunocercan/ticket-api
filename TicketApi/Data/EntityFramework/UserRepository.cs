using Microsoft.EntityFrameworkCore;
using TicketAPI.DataTransferObjects;
using TicketAPI.Interfaces;

namespace TicketAPI.Data.EntityFramework
{
    public class UserRepository(AppDbContext context) : IUserRepository
    {
        private readonly AppDbContext _context = context;

        public async Task<bool> UserExists(int userId)
        {
            return await _context.Users.AnyAsync(u => u.Id == userId);
        }

        public async Task<UsersDto> GetUserByIdAsync(int userId)
        {
            return _context.Users.Where(u => u.Id == userId).Single();
        }

        public async Task<List<UsersDto>> GetUserListAsync()
        {
            return _context.Users.ToList();
        }
    }

}