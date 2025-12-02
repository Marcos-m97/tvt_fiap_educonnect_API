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
            await CreateAdminUserAndProfile();
        }

        private async Task CreateAdminUserAndProfile()
        {
            Console.WriteLine(">>> Entrou no CreateAdminUserAndProfile()");

            var email = _config["Seed:DefaultEmail"];
            var name = _config["Seed:DefaultName"];
            var password = _config["Seed:DefaultPassword"];
            var tipoStr = _config["Seed:DefaultTipo"]; // deve ser "1" para Admin

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                Console.WriteLine("⚠ Seed ignorada: email/senha não configurados.");
                return;
            }

            int tipo = int.Parse(tipoStr ?? "1");

            // 1) Verifica se o USUÁRIO já existe
            var existingUser = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);

            Usuario usuario;

            if (existingUser != null)
            {
                Console.WriteLine("✔ Usuário já existe.");
                usuario = existingUser;
            }
            else
            {
                // cria usuário novo
                var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                usuario = new Usuario
                {
                    Nome = name,
                    Email = email,
                    SenhaHash = hashedPassword,
                    Tipo = tipo,   // deve ser 1 (Admin)
                    CriadoEm = DateTime.Now,
                    Ativo = true
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                Console.WriteLine($"🔥 Usuário ADMIN criado via seed! ID: {usuario.Id}");
            }

            // 2) Criar PERFIL ADMIN caso não exista
            var existingAdminProfile = await _context.Admins
                .FirstOrDefaultAsync(a => a.UsuarioId == usuario.Id);

            if (existingAdminProfile != null)
            {
                Console.WriteLine("✔ Perfil Admin já existe.");
                return;
            }

            var admin = new Admin
            {
                Id = Guid.NewGuid(),
                UsuarioId = usuario.Id,
                Departamento = "Diretoria Acadêmica",
                Cargo = "Administrador do Sistema"
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            Console.WriteLine($"🏆 Perfil ADMIN criado com sucesso! AdminId: {admin.Id}");
        }
    }
}