using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade TurmaDisciplina.
    ///
    /// Essa entidade funciona como uma tabela de associação entre turma, disciplina
    /// e professor. Por isso, as consultas normalmente carregam os relacionamentos
    /// necessários para montar informações completas para o frontend.
    /// </summary>
    public class TurmaDisciplinaRepository : ITurmaDisciplinaRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        /// </summary>
        public TurmaDisciplinaRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR VÍNCULO
        // ============================================================

        /// <summary>
        /// Cria uma nova associação entre turma, disciplina e professor.
        ///
        /// A validação de existência das chaves estrangeiras é feita na camada
        /// de Service. O Repository apenas persiste a entidade recebida.
        /// </summary>
        public async Task<TurmaDisciplina> Criar(TurmaDisciplina entity)
        {
            _context.TurmaDisciplinas.Add(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        // ============================================================
        // 2. OBTER POR ID
        // ============================================================

        /// <summary>
        /// Obtém um vínculo pelo ID, carregando turma, disciplina, professor
        /// e usuário do professor.
        ///
        /// O ThenInclude é usado porque o nome do professor está na entidade Usuario,
        /// vinculada ao Professor.
        /// </summary>
        public async Task<TurmaDisciplina?> ObterPorId(int id)
        {
            return await _context.TurmaDisciplinas
                .Include(td => td.Turma)
                .Include(td => td.Disciplina)
                .Include(td => td.Professor)
                    .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(td => td.Id == id);
        }

        // ============================================================
        // 3. LISTAR TODOS
        // ============================================================

        /// <summary>
        /// Lista todas as associações entre turmas, disciplinas e professores.
        ///
        /// Os Includes permitem retornar dados descritivos no DTO, como nome
        /// da turma, nome da disciplina e nome do professor.
        /// </summary>
        public async Task<IEnumerable<TurmaDisciplina>> Listar()
        {
            return await _context.TurmaDisciplinas
                .Include(td => td.Turma)
                .Include(td => td.Disciplina)
                .Include(td => td.Professor)
                    .ThenInclude(p => p.Usuario)
                .ToListAsync();
        }

        // ============================================================
        // 4. LISTAR POR TURMA
        // ============================================================

        /// <summary>
        /// Lista os vínculos acadêmicos de uma turma específica.
        ///
        /// Esse método é usado para montar a grade da turma, exibindo disciplinas
        /// e professores responsáveis.
        /// </summary>
        public async Task<IEnumerable<TurmaDisciplina>> ListarPorTurma(int turmaId)
        {
            return await _context.TurmaDisciplinas
                .Include(td => td.Turma)
                .Include(td => td.Disciplina)
                .Include(td => td.Professor)
                    .ThenInclude(p => p.Usuario)
                .Where(td => td.TurmaId == turmaId)
                .ToListAsync();
        }

        // ============================================================
        // 5. ATUALIZAR VÍNCULO
        // ============================================================

        /// <summary>
        /// Atualiza uma associação existente.
        ///
        /// A entidade já chega modificada pela camada de Service.
        /// </summary>
        public async Task<TurmaDisciplina> Atualizar(TurmaDisciplina entity)
        {
            _context.TurmaDisciplinas.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }

        // ============================================================
        // 6. DELETAR VÍNCULO
        // ============================================================

        /// <summary>
        /// Remove uma associação entre turma, disciplina e professor.
        ///
        /// Como essa entidade representa um vínculo, e não uma entidade acadêmica
        /// principal, o fluxo atual utiliza exclusão física.
        /// </summary>
        public async Task<bool> Deletar(int id)
        {
            var entity = await _context.TurmaDisciplinas.FindAsync(id);

            if (entity == null)
                return false;

            _context.TurmaDisciplinas.Remove(entity);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============================================================
        // 7. LISTAR POR PROFESSOR
        // ============================================================

        /// <summary>
        /// Lista todas as turmas e disciplinas vinculadas a um professor.
        ///
        /// Esse método é útil para montar a visão do professor, exibindo
        /// quais disciplinas ele ministra e em quais turmas.
        /// </summary>
        public async Task<IEnumerable<TurmaDisciplina>> ListarPorProfessor(int professorId)
        {
            return await _context.TurmaDisciplinas
                .Include(td => td.Turma)
                .Include(td => td.Disciplina)
                .Include(td => td.Professor)
                    .ThenInclude(p => p.Usuario)
                .Where(td => td.ProfessorId == professorId)
                .ToListAsync();
        }
    }
}