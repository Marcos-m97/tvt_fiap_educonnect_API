using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Curso.
    ///
    /// No EduConnect, o curso representa uma estrutura acadêmica principal,
    /// que pode possuir disciplinas, turmas e matrículas relacionadas.
    ///
    /// Essa camada isola o acesso ao banco de dados, mantendo consultas e operações
    /// de persistência fora do Controller e do Service.
    /// </summary>
    public class CursoRepository : ICursoRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext representa a conexão com o banco e expõe a tabela de cursos.
        /// </summary>
        public CursoRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR CURSO
        // ============================================================

        /// <summary>
        /// Cria um novo curso no banco de dados.
        ///
        /// A entidade já chega montada pela camada de Service, que é responsável
        /// por aplicar as regras de negócio antes da persistência.
        /// </summary>
        public async Task<Curso> Criar(Curso curso)
        {
            _context.Cursos.Add(curso);
            await _context.SaveChangesAsync();

            return curso;
        }

        // ============================================================
        // 2. OBTER CURSO POR ID
        // ============================================================

        /// <summary>
        /// Obtém um curso pelo seu identificador único.
        ///
        /// Esse método é usado em fluxos de consulta detalhada, edição,
        /// exclusão lógica e reativação.
        /// </summary>
        public async Task<Curso?> ObterPorId(int id)
        {
            return await _context.Cursos
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // ============================================================
        // 3. LISTAR TODOS
        // ============================================================

        /// <summary>
        /// Lista todos os cursos cadastrados.
        ///
        /// Esse método retorna a lista completa sem paginação.
        /// No fluxo atual, a listagem paginada é feita pelo Service usando Query().
        /// </summary>
        public async Task<IEnumerable<Curso>> Listar()
        {
            return await _context.Cursos.ToListAsync();
        }

        // ============================================================
        // 4. ATUALIZAR CURSO
        // ============================================================

        /// <summary>
        /// Atualiza os dados de um curso existente no banco.
        ///
        /// A entidade já chega modificada pela camada de Service.
        /// </summary>
        public async Task<Curso> Atualizar(Curso curso)
        {
            _context.Cursos.Update(curso);
            await _context.SaveChangesAsync();

            return curso;
        }

        // ============================================================
        // 5. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente um curso.
        ///
        /// Em vez de remover o registro fisicamente do banco, o sistema altera
        /// o campo Ativo para false.
        ///
        /// Essa abordagem preserva o histórico acadêmico relacionado ao curso,
        /// como turmas, disciplinas e matrículas já existentes.
        /// </summary>
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

        // ============================================================
        // 6. REATIVAR CURSO
        // ============================================================

        /// <summary>
        /// Reativa um curso previamente desativado.
        ///
        /// Esse método altera o campo Ativo para true, permitindo que o curso
        /// volte a ser exibido e utilizado nos fluxos acadêmicos.
        /// </summary>
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

        // ============================================================
        // 7. QUERY BASE
        // ============================================================

        /// <summary>
        /// Retorna uma consulta base de cursos como IQueryable.
        ///
        /// Esse método permite que a camada de Service aplique filtros,
        /// ordenação e paginação antes de executar a consulta no banco.
        ///
        /// No fluxo de listagem, isso evita carregar todos os cursos em memória
        /// antes de aplicar paginação.
        /// </summary>
        public IQueryable<Curso> Query()
        {
            return _context.Cursos.AsQueryable();
        }
    }
}