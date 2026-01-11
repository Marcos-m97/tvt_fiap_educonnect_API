using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class EntregaRepository : IEntregaRepository
    {
        private readonly AppDbContext _context;

        public EntregaRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<EntregaAtividade> Criar(EntregaAtividade entrega)
        {
            _context.EntregasAtividades.Add(entrega);
            await _context.SaveChangesAsync();
            return entrega;
        }

        public async Task<EntregaAtividade?> ObterPorId(int id)
        {
            return await _context.EntregasAtividades
                .Include(e => e.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(e => e.Atividade)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<EntregaAtividade>> ListarPorAtividade(int atividadeId)
        {
            return await _context.EntregasAtividades
                .Where(e => e.AtividadeId == atividadeId)
                .Include(e => e.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(e => e.Atividade)
                .ToListAsync();
        }

        public async Task<EntregaAtividade> Atualizar(EntregaAtividade entrega)
        {
            _context.EntregasAtividades.Update(entrega);
            await _context.SaveChangesAsync();
            return entrega;
        }
        public async Task<IEnumerable<EntregaAtividade>> ListarPorAluno(int alunoId)
        {
            return await _context.EntregasAtividades
                .Where(e => e.AlunoId == alunoId)
                .Include(e => e.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(e => e.Atividade)
                    .ThenInclude(a => a.TurmaDisciplina)
                        .ThenInclude(td => td.Disciplina)
                .ToListAsync();
        }

    }
}