using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class AtividadeRepository : IAtividadeRepository
    {
        private readonly AppDbContext _context;

        public AtividadeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Atividade> Criar(Atividade atividade)
        {
            _context.Atividades.Add(atividade);
            await _context.SaveChangesAsync();
            return atividade;
        }

        public async Task<Atividade?> ObterPorId(Guid id)
        {
            return await _context.Atividades
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Turma)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Professor)
                        .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Atividade>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId)
        {
            return await _context.Atividades
                .Where(a => a.TurmaDisciplinaId == turmaDisciplinaId)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Turma)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Professor)
                        .ThenInclude(p => p.Usuario)
                .ToListAsync();
        }
    }
}