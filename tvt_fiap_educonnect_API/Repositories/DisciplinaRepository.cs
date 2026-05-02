using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Disciplina.
    ///
    /// No EduConnect, essa camada centraliza as consultas e alterações relacionadas
    /// às disciplinas, utilizando Entity Framework.
    ///
    /// Os métodos utilizam Include em Curso porque o DTO de disciplina retorna
    /// também o nome do curso vinculado.
    /// </summary>
    public class DisciplinaRepository : IDisciplinaRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        /// </summary>
        public DisciplinaRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR DISCIPLINA
        // ============================================================

        /// <summary>
        /// Cria uma nova disciplina no banco de dados.
        ///
        /// A validação de existência do curso é feita na camada de Service.
        /// O Repository apenas persiste a entidade recebida.
        /// </summary>
        public async Task<Disciplina> Criar(Disciplina disciplina)
        {
            _context.Disciplinas.Add(disciplina);
            await _context.SaveChangesAsync();

            return disciplina;
        }

        // ============================================================
        // 2. LISTAR DISCIPLINAS
        // ============================================================

        /// <summary>
        /// Lista todas as disciplinas cadastradas, incluindo ativas e inativas.
        ///
        /// Essa escolha permite que o painel administrativo consiga exibir
        /// registros desativados e oferecer a opção de reativação.
        ///
        /// O Include em Curso permite retornar o nome do curso no DTO.
        /// </summary>
        public async Task<IEnumerable<Disciplina>> Listar()
        {
            return await _context.Disciplinas
                .Include(d => d.Curso)
                .ToListAsync();
        }

        // ============================================================
        // 3. OBTER DISCIPLINA POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma disciplina pelo ID, incluindo o curso relacionado.
        ///
        /// A consulta não filtra por Ativo para permitir que o sistema também
        /// consulte disciplinas inativas em telas administrativas ou de reativação.
        /// </summary>
        public async Task<Disciplina?> ObterPorId(int id)
        {
            return await _context.Disciplinas
                .Include(d => d.Curso)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        // ============================================================
        // 4. LISTAR DISCIPLINAS POR CURSO
        // ============================================================

        /// <summary>
        /// Lista as disciplinas vinculadas a um curso específico.
        ///
        /// Essa consulta é usada quando o frontend precisa carregar a grade
        /// acadêmica de acordo com o curso selecionado.
        ///
        /// A consulta também não filtra por Ativo, permitindo que o painel
        /// administrativo visualize disciplinas desativadas relacionadas ao curso.
        /// </summary>
        public async Task<IEnumerable<Disciplina>> ListarPorCurso(int cursoId)
        {
            return await _context.Disciplinas
                .Include(d => d.Curso)
                .Where(d => d.CursoId == cursoId)
                .ToListAsync();
        }

        // ============================================================
        // 5. ATUALIZAR DISCIPLINA
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma disciplina existente.
        ///
        /// A entidade já chega alterada pela camada de Service.
        /// </summary>
        public async Task<Disciplina> Atualizar(Disciplina disciplina)
        {
            _context.Disciplinas.Update(disciplina);
            await _context.SaveChangesAsync();

            return disciplina;
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente uma disciplina.
        ///
        /// Em vez de remover fisicamente o registro do banco, o campo Ativo
        /// é alterado para false.
        ///
        /// Essa abordagem preserva histórico acadêmico e evita impacto em vínculos
        /// com curso, turmas, aulas, atividades ou registros já existentes.
        /// </summary>
        public async Task<bool> Deletar(int id)
        {
            var disciplina = await _context.Disciplinas.FindAsync(id);

            if (disciplina == null)
                return false;

            disciplina.Ativo = false;

            _context.Disciplinas.Update(disciplina);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}