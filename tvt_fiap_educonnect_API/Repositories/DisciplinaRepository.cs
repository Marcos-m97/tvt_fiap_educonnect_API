using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class DisciplinaRepository : IDisciplinaRepository
    {
        private readonly AppDbContext _context;

        public DisciplinaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Disciplina> Criar(Disciplina disciplina)
        {
            _context.Disciplinas.Add(disciplina);
            await _context.SaveChangesAsync();
            return disciplina;
        }

        public async Task<IEnumerable<Disciplina>> Listar()
        {
            return await _context.Disciplinas
                .Include(d => d.Curso)
                .ToListAsync();
        }

        public async Task<Disciplina?> ObterPorId(int id)
        {
            return await _context.Disciplinas
                .Include(d => d.Curso)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Disciplina>> ListarPorCurso(int cursoId)
        {
            return await _context.Disciplinas
                .Include(d => d.Curso)
                .Where(d => d.CursoId == cursoId)
                .ToListAsync();
        }

        public async Task<Disciplina> Atualizar(Disciplina disciplina)
        {
            _context.Disciplinas.Update(disciplina);
            await _context.SaveChangesAsync();
            return disciplina;
        }

        public async Task<bool> Deletar(int id)
        {
            var disciplina = await _context.Disciplinas.FindAsync(id);
            if (disciplina == null)
                return false;

            _context.Disciplinas.Remove(disciplina);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}