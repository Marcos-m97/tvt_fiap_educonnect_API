using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class TurmaRepository : ITurmaRepository
    {
        private readonly AppDbContext _context;

        public TurmaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Turma> Criar(Turma turma)
        {
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();
            return turma;
        }

        // 🔥 NÃO FILTRA MAIS POR ATIVO (igual Disciplina)
        public async Task<Turma?> ObterPorId(int id)
        {
            return await _context.Turmas
                .Include(t => t.Curso)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Turma>> Listar()
        {
            return await _context.Turmas
                .Include(t => t.Curso)
                .ToListAsync();
        }

        public async Task<IEnumerable<Turma>> ListarPorCurso(int cursoId)
        {
            return await _context.Turmas
                .Include(t => t.Curso)
                .Where(t => t.CursoId == cursoId)
                .ToListAsync();
        }

        public async Task<Turma> Atualizar(Turma turma)
        {
            _context.Turmas.Update(turma);
            await _context.SaveChangesAsync();
            return turma;
        }

        // 🔥 SOFT DELETE
        public async Task<bool> Deletar(int id)
        {
            var turma = await _context.Turmas.FindAsync(id);
            if (turma == null)
                return false;

            turma.Ativo = false;

            _context.Turmas.Update(turma);
            await _context.SaveChangesAsync();

            return true;
        }

        // 🔥 REATIVAR
        public async Task<bool> Reativar(int id)
        {
            var turma = await _context.Turmas.FindAsync(id);
            if (turma == null)
                return false;

            turma.Ativo = true;

            _context.Turmas.Update(turma);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}