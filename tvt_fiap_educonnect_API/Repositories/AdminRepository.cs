using Microsoft.EntityFrameworkCore;
using EduConnect_API.Data;
using EduConnect_API.Models;
namespace EduConnect_API.Repositories.Interfaces
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _context;

        public AdminRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Admin> Criar(Admin admin)
        {
            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();
            return admin;
        }

        public async Task<Admin?> ObterPorUsuarioId(Guid usuarioId)
        {
            return await _context.Admins
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId);
        }

        public async Task<Admin?> ObterPorId(Guid id)
        {
            return await _context.Admins
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Admin>> Listar()
        {
            return await _context.Admins
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        public async Task<Admin> Atualizar(Admin admin)
        {
            _context.Admins.Update(admin);
            await _context.SaveChangesAsync();
            return admin;
        }
    }
}
