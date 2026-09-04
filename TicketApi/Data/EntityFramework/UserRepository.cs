using Microsoft.EntityFrameworkCore;
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
    }
}