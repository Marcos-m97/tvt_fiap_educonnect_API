using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class AulaRepository : IAulaRepository
    {
        private readonly AppDbContext _context;

        public AulaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Aula> Criar(Aula aula)
        {
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();
            return aula;
        }

        public async Task<Aula?> ObterPorId(Guid id)
        {
            return await _context.Aulas
                .Include(a => a.TurmaDisciplina)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Aula>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId)
        {
            return await _context.Aulas
                .Where(a => a.TurmaDisciplinaId == turmaDisciplinaId)
                .OrderBy(a => a.CriadoEm)
                .ToListAsync();
        }

        public async Task<IEnumerable<Aula>> Listar()
        {
            return await _context.Aulas
                .OrderByDescending(a => a.CriadoEm)
                .ToListAsync();
        }

        public async Task<Aula> Atualizar(Aula aula)
        {
            _context.Aulas.Update(aula);
            await _context.SaveChangesAsync();
            return aula;
        }

        public async Task<bool> Deletar(Guid id)
        {
            var aula = await _context.Aulas.FindAsync(id);
            if (aula == null)
                return false;

            _context.Aulas.Remove(aula);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}