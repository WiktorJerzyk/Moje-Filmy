using Microsoft.EntityFrameworkCore;

namespace Project
{
    public class AppDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; } // Tabela w bazie danych

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    }
}
