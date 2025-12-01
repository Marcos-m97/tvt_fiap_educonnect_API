using EduConnect_API.Models;
using Microsoft.EntityFrameworkCore;
using tvt_fiap_educonnect_API.Models;

namespace EduConnect_API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}
        
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<PasswordResetCode> PasswordResetCodes { get; set; }
    }
}
