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
        public async Task<IEnumerable<Matricula>> ListarPorTurma(int turmaId)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno).ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .Where(m => m.TurmaId == turmaId)
                .ToListAsync();
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