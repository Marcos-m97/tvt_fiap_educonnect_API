using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    public class EventoRepository : IEventoRepository
    {
        private readonly AppDbContext _context;

        public EventoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Evento> Criar(Evento evento)
        {
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task<Evento?> Obter(Guid id)
        {
            return await _context.Eventos
                .Include(e => e.Turma)
                .Include(e => e.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Evento>> Listar()
        {
            return await _context.Eventos
                .Include(e => e.Turma)
                .Include(e => e.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> ListarPorTurma(Guid turmaId)
        {
            return await _context.Eventos
                .Where(e => e.TurmaId == turmaId)
                .Include(e => e.Turma)
                .Include(e => e.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .ToListAsync();
        }

        public async Task<IEnumerable<Evento>> ListarPorUsuario(Guid usuarioId)
        {
            return await _context.Eventos
                .Where(e => e.CriadoPorId == usuarioId)
                .Include(e => e.Turma)
                .Include(e => e.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .ToListAsync();
        }

        public async Task<Evento> Atualizar(Evento evento)
        {
            _context.Eventos.Update(evento);
            await _context.SaveChangesAsync();
            return evento;
        }

        public async Task<bool> Deletar(Guid id)
        {
            var e = await _context.Eventos.FindAsync(id);
            if (e == null) return false;

            _context.Eventos.Remove(e);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
