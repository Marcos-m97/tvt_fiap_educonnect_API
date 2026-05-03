using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Evento.
    ///
    /// No EduConnect, eventos representam compromissos acadêmicos ou administrativos,
    /// como provas, atividades, aulas extras, reuniões ou avisos gerais.
    ///
    /// Essa camada centraliza criação, consulta, listagem, atualização e remoção
    /// de eventos, além de carregar os relacionamentos necessários para exibição
    /// no frontend.
    /// </summary>
    public class EventoRepository : IEventoRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext expõe a tabela de eventos e seus relacionamentos
        /// opcionais com turma e turma/disciplina.
        /// </summary>
        public EventoRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR EVENTO
        // ============================================================

        /// <summary>
        /// Cria um novo evento no banco de dados.
        ///
        /// A entidade já chega montada pela camada de Service, que é responsável
        /// por definir o usuário criador e validar os vínculos acadêmicos.
        /// </summary>
        public async Task<Evento> Criar(Evento evento)
        {
            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            return evento;
        }

        // ============================================================
        // 2. OBTER EVENTO POR ID
        // ============================================================

        /// <summary>
        /// Obtém um evento específico pelo ID.
        ///
        /// A consulta carrega a turma e, quando existir, a TurmaDisciplina
        /// com sua disciplina relacionada. Isso permite que o frontend exiba
        /// informações descritivas do contexto acadêmico do evento.
        /// </summary>
        public async Task<Evento?> Obter(int id)
        {
            return await _context.Eventos
                .Include(e => e.Turma)
                .Include(e => e.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // ============================================================
        // 3. LISTAR EVENTOS
        // ============================================================

        /// <summary>
        /// Lista todos os eventos cadastrados.
        ///
        /// A consulta inclui turma e disciplina para permitir que a camada
        /// de Service monte DTOs completos para exibição em calendário ou listagens.
        /// </summary>
        public async Task<IEnumerable<Evento>> Listar()
        {
            return await _context.Eventos
                .Include(e => e.Turma)
                .Include(e => e.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .ToListAsync();
        }

        // ============================================================
        // 4. LISTAR EVENTOS POR TURMA
        // ============================================================

        /// <summary>
        /// Lista os eventos relacionados a uma turma específica.
        ///
        /// A regra considera dois cenários:
        /// 1. eventos vinculados diretamente à turma;
        /// 2. eventos vinculados a uma TurmaDisciplina pertencente à turma.
        ///
        /// Isso permite que o aluno visualize tanto eventos gerais da turma
        /// quanto eventos específicos de disciplinas daquela turma.
        /// </summary>
        public async Task<IEnumerable<Evento>> ListarPorTurma(int turmaId)
        {
            return await _context.Eventos
                .Where(e =>
                    e.TurmaId == turmaId ||
                    (e.TurmaDisciplina != null &&
                     e.TurmaDisciplina.TurmaId == turmaId)
                )
                .Include(e => e.Turma)
                .Include(e => e.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .ToListAsync();
        }

        // ============================================================
        // 5. LISTAR EVENTOS POR USUÁRIO
        // ============================================================

        /// <summary>
        /// Lista os eventos criados por um usuário específico.
        ///
        /// Esse método pode ser usado para exibir eventos cadastrados por determinado
        /// professor ou administrador.
        /// </summary>
        public async Task<IEnumerable<Evento>> ListarPorUsuario(int usuarioId)
        {
            return await _context.Eventos
                .Where(e => e.CriadoPorId == usuarioId)
                .Include(e => e.Turma)
                .Include(e => e.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .ToListAsync();
        }

        // ============================================================
        // 6. ATUALIZAR EVENTO
        // ============================================================

        /// <summary>
        /// Atualiza os dados de um evento existente.
        ///
        /// A entidade já chega modificada pela camada de Service.
        /// </summary>
        public async Task<Evento> Atualizar(Evento evento)
        {
            _context.Eventos.Update(evento);
            await _context.SaveChangesAsync();

            return evento;
        }

        // ============================================================
        // 7. DELETAR EVENTO
        // ============================================================

        /// <summary>
        /// Remove um evento do banco de dados.
        ///
        /// No fluxo atual, eventos são removidos fisicamente quando excluídos.
        /// Caso o evento não exista, retorna false para que o Controller responda
        /// com NotFound.
        /// </summary>
        public async Task<bool> Deletar(int id)
        {
            var e = await _context.Eventos.FindAsync(id);

            if (e == null)
                return false;

            _context.Eventos.Remove(e);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}