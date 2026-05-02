using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Turma.
    ///
    /// No EduConnect, essa camada centraliza as consultas e alterações relacionadas
    /// às turmas, utilizando Entity Framework.
    ///
    /// Os métodos utilizam Include em Curso porque o DTO de turma retorna também
    /// o nome do curso vinculado.
    /// </summary>
    public class TurmaRepository : ITurmaRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        /// </summary>
        public TurmaRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR TURMA
        // ============================================================

        /// <summary>
        /// Cria uma nova turma no banco de dados.
        ///
        /// A validação de existência do curso é feita na camada de Service.
        /// O Repository apenas persiste a entidade recebida.
        /// </summary>
        public async Task<Turma> Criar(Turma turma)
        {
            _context.Turmas.Add(turma);
            await _context.SaveChangesAsync();

            return turma;
        }

        // ============================================================
        // 2. OBTER TURMA POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma turma pelo ID, incluindo o curso relacionado.
        ///
        /// A consulta não filtra por Ativo para permitir que telas administrativas
        /// também consultem turmas inativas para visualização ou reativação.
        /// </summary>
        public async Task<Turma?> ObterPorId(int id)
        {
            return await _context.Turmas
                .Include(t => t.Curso)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        // ============================================================
        // 3. LISTAR TURMAS
        // ============================================================

        /// <summary>
        /// Lista todas as turmas cadastradas, incluindo ativas e inativas.
        ///
        /// Essa escolha permite que o painel administrativo visualize registros
        /// desativados e ofereça a opção de reativação.
        ///
        /// O Include em Curso permite retornar o nome do curso no DTO.
        /// </summary>
        public async Task<IEnumerable<Turma>> Listar()
        {
            return await _context.Turmas
                .Include(t => t.Curso)
                .ToListAsync();
        }

        // ============================================================
        // 4. LISTAR TURMAS POR CURSO
        // ============================================================

        /// <summary>
        /// Lista as turmas vinculadas a um curso específico.
        ///
        /// Essa consulta é usada quando o frontend precisa carregar turmas
        /// a partir de um curso selecionado, por exemplo no processo de matrícula.
        /// </summary>
        public async Task<IEnumerable<Turma>> ListarPorCurso(int cursoId)
        {
            return await _context.Turmas
                .Include(t => t.Curso)
                .Where(t => t.CursoId == cursoId)
                .ToListAsync();
        }

        // ============================================================
        // 5. ATUALIZAR TURMA
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma turma existente.
        ///
        /// A entidade já chega alterada pela camada de Service.
        /// </summary>
        public async Task<Turma> Atualizar(Turma turma)
        {
            _context.Turmas.Update(turma);
            await _context.SaveChangesAsync();

            return turma;
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente uma turma.
        ///
        /// Em vez de remover fisicamente o registro, o campo Ativo é alterado
        /// para false.
        ///
        /// Essa abordagem preserva histórico de matrículas, disciplinas associadas
        /// e dados acadêmicos vinculados à turma.
        /// </summary>
        public async Task<bool> Deletar(int id)
        {
            var turma = await _context.Turmas.FindAsync(id);

            if (turma == null)
                return false;

            turma.Ativo = false;

            _context.Turmas.Update(turma);
            await _context.SaveChangesAsync();

            return true;
        }

        // ============================================================
        // 7. REATIVAR TURMA
        // ============================================================

        /// <summary>
        /// Reativa uma turma previamente desativada.
        ///
        /// O campo Ativo é alterado novamente para true, permitindo que a turma
        /// volte a ser utilizada nos fluxos acadêmicos.
        /// </summary>
        public async Task<bool> Reativar(int id)
        {
            var turma = await _context.Turmas.FindAsync(id);

            if (turma == null)
                return false;

            turma.Ativo = true;

            _context.Turmas.Update(turma);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}