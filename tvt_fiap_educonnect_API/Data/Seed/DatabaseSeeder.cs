using EduConnect_API.Data;
using EduConnect_API.Models;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Data.Seed
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public DatabaseSeeder(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task SeedAsync()
        {
            Console.WriteLine(">>> SEED INICIADA <<<");
            await CreateAdminUser();
        }

        private async Task CreateAdminUser()
        {
            Console.WriteLine(">>> Entrou no CreateAdminUser()");

            var email = _config["Seed:DefaultEmail"];
            var name = _config["Seed:DefaultName"];
            var password = _config["Seed:DefaultPassword"];
            var tipoStr = _config["Seed:DefaultTipo"];

            Console.WriteLine($"Email -> {email}");
            Console.WriteLine($"Password -> {password}");
            Console.WriteLine($"Tipo -> {tipoStr}");

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("⚠ Seed ignorada: email/senha não configurados.");
                return;
            }

            int tipo = int.Parse(tipoStr ?? "0");

            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            if (existingUser != null)
            {
                Console.WriteLine("✔ Admin já existe. Seed ignorada.");
                return;
            }

            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            var admin = new Usuario
            {
                Nome = name,
                Email = email,
                SenhaHash = hashedPassword,
                Tipo = tipo,
                CriadoEm = DateTime.Now
            };

            _context.Add(admin);
            await _context.SaveChangesAsync();

            Console.WriteLine($"🔥 Admin criado via seed com sucesso! ID: {admin.Id}");

        }
    }
}