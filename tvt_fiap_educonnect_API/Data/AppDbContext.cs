using Microsoft.EntityFrameworkCore;
using EduConnect_API.Models;

namespace EduConnect_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Usuario> Usuarios { get; set; }
    }
}
