using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class CursoRepository : ICursoRepository
    {
        private readonly AppDbContext _context;

        public CursoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Curso> Criar(Curso curso)
        {
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();
            return curso;
        }

        public async Task<Curso?> ObterPorId(int id)
        {
            return await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<Curso>> Listar()
        {
            return await _context.Cursos.ToListAsync();
        }

        public async Task<Curso> Atualizar(Curso curso)
        {
            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();
            return curso;
        }

        // 🔥 SOFT DELETE
        public async Task<bool> Deletar(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return false;

            curso.Ativo = false;

            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();

            return true;
        }

        // 🔥 REATIVAR
        public async Task<bool> Reativar(int id)
        {
            var curso = await _context.Cursos.FindAsync(id);
            if (curso == null)
                return false;

            curso.Ativo = true;

            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();

            return true;
        }

        public IQueryable<Curso> Query()
        {
            return _context.Cursos.AsQueryable();
        }
    }
}