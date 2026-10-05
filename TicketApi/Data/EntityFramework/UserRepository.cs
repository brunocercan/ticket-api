using Microsoft.EntityFrameworkCore;
using TicketAPI.DataTransferObjects;
using TicketAPI.Interfaces;
using TicketAPI.Helpers;

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

        public async Task<PagedList<UsersDto>> GetUserListAsync(int pageNumber, int pageSize)
        {
            var query = _context.Users.AsQueryable();
            return await PagedList<UsersDto>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task<UsersDto?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CreateUserAsync(UsersDto user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserAsync(int userId, UsersDto user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
        }
    }
}