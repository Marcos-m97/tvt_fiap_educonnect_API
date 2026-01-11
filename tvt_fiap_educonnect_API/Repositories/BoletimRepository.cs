using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class BoletimRepository : IBoletimRepository
    {
        private readonly AppDbContext _context;

        public BoletimRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Boletim> Criar(Boletim boletim)
        {
            _context.Boletins.Add(boletim);
            await _context.SaveChangesAsync();
            return boletim;
        }

        public async Task<Boletim?> Obter(int id)
        {
            return await _context.Boletins
                .Include(b => b.Disciplinas)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<IEnumerable<Boletim>> ListarPorAluno(int alunoId)
        {
            return await _context.Boletins
                .Include(b => b.Disciplinas)
                .Where(b => b.AlunoId == alunoId)
                .ToListAsync();
        }
    }
}
