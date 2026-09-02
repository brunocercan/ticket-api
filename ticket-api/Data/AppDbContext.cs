using Microsoft.EntityFrameworkCore;
using DataTransferObjects;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<CategoriesDto> Categories { get; set; }
        public DbSet<TicketsDto> Tickets { get; set; }
        public DbSet<UsersDto> Users { get; set; }
        public DbSet<TicketCommentsDto> TicketComments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<CategoriesDto>()
                .HasIndex(c => c.Id)
                .IsUnique();
        }
    }
}
