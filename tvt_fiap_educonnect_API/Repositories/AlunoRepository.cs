using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class AlunoRepository : IAlunoRepository
    {
        private readonly AppDbContext _context;

        public AlunoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Aluno> Criar(Aluno aluno)
        {
            _context.Alunos.Add(aluno);
            await _context.SaveChangesAsync();
            return aluno;
        }

        public async Task<Aluno?> ObterPorUsuarioId(Guid usuarioId)
        {
            return await _context.Alunos
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.UsuarioId == usuarioId);
        }

        public async Task<Aluno?> ObterPorId(Guid id)
        {
            return await _context.Alunos
                .Include(a => a.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<Aluno>> Listar()
        {
            return await _context.Alunos
                .Include(a => a.Usuario)
                .ToListAsync();
        }

        public async Task<Aluno> Atualizar(Aluno aluno)
        {
            _context.Alunos.Update(aluno);
            await _context.SaveChangesAsync();
            return aluno;
        }
    }
}