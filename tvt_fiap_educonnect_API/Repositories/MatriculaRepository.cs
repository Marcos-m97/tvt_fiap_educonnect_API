using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class MatriculaRepository : IMatriculaRepository
    {
        private readonly AppDbContext _context;

        public MatriculaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Matricula> Criar(Matricula matricula)
        {
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();
            return matricula;
        }

        public async Task<Matricula?> ObterPorId(int id)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno).ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task<IEnumerable<Matricula>> Listar()
        {
            return await _context.Matriculas
                .Include(m => m.Aluno).ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .ToListAsync();
        }

        public async Task<IEnumerable<Matricula>> ListarPorAluno(int alunoId)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno).ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .Where(m => m.AlunoId == alunoId)
                .ToListAsync();
        }

        // 🔹 MÉTODO ANTIGO (mantido para não quebrar a interface)
        public async Task<IEnumerable<Matricula>> ListarPorTurma(int turmaId)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno).ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .Where(m => m.TurmaId == turmaId)
                .ToListAsync();
        }

        // 🔹 NOVO MÉTODO PAGINADO
        public async Task<(IEnumerable<Matricula> Items, int TotalCount)>
            ListarAlunosPorTurmaPaginado(
                int turmaId,
                int page,
                int pageSize,
                string? search)
        {
            var query = _context.Matriculas
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .Where(m =>
                    m.TurmaId == turmaId &&
                    m.Status == MatriculaStatus.Efetivada)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();
                query = query.Where(m =>
                    m.Aluno.Usuario.Nome.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.Aluno.Usuario.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        public async Task<Matricula> Atualizar(Matricula matricula)
        {
            _context.Matriculas.Update(matricula);
            await _context.SaveChangesAsync();
            return matricula;
        }

        public async Task<bool> Deletar(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);
            if (matricula == null)
                return false;

            _context.Matriculas.Remove(matricula);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Matricula?> ObterAtivaPorAlunoId(int alunoId)
        {
            return await _context.Matriculas
                .Include(m => m.Turma)
                    .ThenInclude(t => t.Curso)
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .FirstOrDefaultAsync(m =>
                    m.AlunoId == alunoId &&
                    m.Status == MatriculaStatus.Efetivada);
        }
    }
}