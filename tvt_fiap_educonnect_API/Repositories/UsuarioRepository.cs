using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. OBTER POR EMAIL (LOGIN)
        // ============================================================
        public async Task<Usuario?> ObterPorEmail(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        // ============================================================
        // 2. OBTER POR ID (/ME)
        // ============================================================
        public async Task<Usuario?> ObterPorId(int id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        // ============================================================
        // 3. CRIAR USUÁRIO 
        // ============================================================
        public async Task<Usuario> Criar(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // GET ALL USUARIOS
        public async Task<IEnumerable<Usuario>> ListarTodos()
        {
            return await _context.Usuarios
                .Where(u => u.Ativo)
                .ToListAsync();
        }

        // ATUALIZAR USUARIOS
        public async Task<Usuario> Atualizar(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // SOFT DELETE
        public async Task<bool> SoftDelete(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return false;

            usuario.Ativo = false;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return true;
        }
        // reativar usuario
        public async Task<bool> Reativar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return false;

            usuario.Ativo = true;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return true;
        }


    }
}
