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

        // ============================================================
        // 4. LISTAR PAGINADO + BUSCA
        // ============================================================
        public async Task<(IEnumerable<Usuario>, int)> ListarPaginado(
            int page,
            int pageSize,
            string? search)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Nome.Contains(search) ||
                    u.Email.Contains(search) ||
                    u.Id.ToString().Contains(search));
            }

            query = query
                .OrderByDescending(u => u.Ativo)
                .ThenBy(u => u.Nome);

            var total = await query.CountAsync();

            var usuarios = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (usuarios, total);
        }

        // ============================================================
        // 5. ATUALIZAR
        // ============================================================
        public async Task<Usuario> Atualizar(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================
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

        // ============================================================
        // 7. REATIVAR
        // ============================================================
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
