using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class ProfessorRepository : IProfessorRepository
    {
        private readonly AppDbContext _context;

        public ProfessorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Professor> Criar(Professor professor)
        {
            _context.Professores.Add(professor);
            await _context.SaveChangesAsync();
            return professor;
        }

        public async Task<Professor?> ObterPorUsuarioId(Guid usuarioId)
        {
            return await _context.Professores
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.UsuarioId == usuarioId);
        }

        public async Task<Professor?> ObterPorId(Guid id)
        {
            return await _context.Professores
                .Include(p => p.Usuario)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Professor>> Listar()
        {
            return await _context.Professores
                .Include(p => p.Usuario)
                .ToListAsync();
        }

        public async Task<Professor> Atualizar(Professor professor)
        {
            _context.Professores.Update(professor);
            await _context.SaveChangesAsync();
            return professor;
        }
    }
}
