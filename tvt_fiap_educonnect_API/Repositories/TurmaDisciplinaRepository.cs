using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class TurmaDisciplinaRepository : ITurmaDisciplinaRepository
    {
        private readonly AppDbContext _context;

        public TurmaDisciplinaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<TurmaDisciplina> Criar(TurmaDisciplina entity)
        {
            _context.TurmaDisciplinas.Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<TurmaDisciplina?> ObterPorId(Guid id)
        {
            return await _context.TurmaDisciplinas
                .Include(td => td.Turma)
                .Include(td => td.Disciplina)
                .Include(td => td.Professor)
                    .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(td => td.Id == id);
        }

        public async Task<IEnumerable<TurmaDisciplina>> Listar()
        {
            return await _context.TurmaDisciplinas
                .Include(td => td.Turma)
                .Include(td => td.Disciplina)
                .Include(td => td.Professor)
                    .ThenInclude(p => p.Usuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<TurmaDisciplina>> ListarPorTurma(Guid turmaId)
        {
            return await _context.TurmaDisciplinas
                .Include(td => td.Turma)
                .Include(td => td.Disciplina)
                .Include(td => td.Professor)
                    .ThenInclude(p => p.Usuario)
                .Where(td => td.TurmaId == turmaId)
                .ToListAsync();
        }

        public async Task<TurmaDisciplina> Atualizar(TurmaDisciplina entity)
        {
            _context.TurmaDisciplinas.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public async Task<bool> Deletar(Guid id)
        {
            var entity = await _context.TurmaDisciplinas.FindAsync(id);
            if (entity == null)
                return false;

            _context.TurmaDisciplinas.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
