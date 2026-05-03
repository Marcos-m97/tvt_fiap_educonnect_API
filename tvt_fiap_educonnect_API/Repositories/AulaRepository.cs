using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Aula.
    ///
    /// No EduConnect, a aula representa um conteúdo acadêmico vinculado a uma
    /// TurmaDisciplina, ou seja, a uma disciplina ofertada dentro de uma turma
    /// com um professor responsável.
    ///
    /// Essa camada centraliza consultas, criação, atualização e remoção de aulas
    /// utilizando Entity Framework.
    /// </summary>
    public class AulaRepository : IAulaRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext expõe a tabela de aulas e seus relacionamentos
        /// com TurmaDisciplina, Turma e Disciplina.
        /// </summary>
        public AulaRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR AULA
        // ============================================================

        /// <summary>
        /// Cria uma nova aula no banco de dados.
        ///
        /// A entidade já chega montada pela camada de Service, que é responsável
        /// por validar o contexto acadêmico e preparar os dados antes da persistência.
        /// </summary>
        public async Task<Aula> Criar(Aula aula)
        {
            _context.Aulas.Add(aula);
            await _context.SaveChangesAsync();

            return aula;
        }

        // ============================================================
        // 2. OBTER AULA POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma aula pelo ID, carregando o contexto acadêmico relacionado.
        ///
        /// O Include em TurmaDisciplina permite acessar informações da associação
        /// entre turma, disciplina e professor.
        ///
        /// Os ThenInclude em Turma e Disciplina permitem retornar dados descritivos,
        /// como nome da turma e nome da disciplina, quando necessário no DTO.
        /// </summary>
        public async Task<Aula?> ObterPorId(int id)
        {
            return await _context.Aulas
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Turma)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // ============================================================
        // 3. LISTAR AULAS POR TURMA/DISCIPLINA
        // ============================================================

        /// <summary>
        /// Lista as aulas vinculadas a uma TurmaDisciplina específica.
        ///
        /// Esse método é usado principalmente na visão da disciplina dentro
        /// de uma turma, exibindo as aulas cadastradas pelo professor para aquele
        /// contexto acadêmico.
        ///
        /// A ordenação por data de criação mantém as aulas na sequência em que
        /// foram cadastradas.
        /// </summary>
        public async Task<IEnumerable<Aula>> ListarPorTurmaDisciplina(int turmaDisciplinaId)
        {
            return await _context.Aulas
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .Where(a => a.TurmaDisciplinaId == turmaDisciplinaId)
                .OrderBy(a => a.CriadoEm)
                .ToListAsync();
        }

        // ============================================================
        // 4. LISTAR AULAS POR TURMA
        // ============================================================

        /// <summary>
        /// Lista todas as aulas disponíveis para uma turma.
        ///
        /// Esse método é usado na jornada do aluno, pois após a matrícula efetivada
        /// o aluno acessa as aulas relacionadas à sua turma.
        ///
        /// A consulta busca aulas por meio da relação TurmaDisciplina, filtrando
        /// pelo ID da turma.
        /// </summary>
        public async Task<IEnumerable<Aula>> ListarPorTurma(int turmaId)
        {
            return await _context.Aulas
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .Where(a => a.TurmaDisciplina.TurmaId == turmaId)
                .OrderBy(a => a.CriadoEm)
                .ToListAsync();
        }

        // ============================================================
        // 5. LISTAR TODAS AS AULAS
        // ============================================================

        /// <summary>
        /// Lista todas as aulas cadastradas, ordenando da mais recente para a mais antiga.
        ///
        /// Esse método pode ser utilizado em contextos administrativos ou listagens gerais.
        /// </summary>
        public async Task<IEnumerable<Aula>> Listar()
        {
            return await _context.Aulas
                .OrderByDescending(a => a.CriadoEm)
                .ToListAsync();
        }

        // ============================================================
        // 6. ATUALIZAR AULA
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma aula existente.
        ///
        /// A entidade já chega modificada pela camada de Service, por exemplo
        /// com novo título, descrição, vídeo ou material de apoio.
        /// </summary>
        public async Task<Aula> Atualizar(Aula aula)
        {
            _context.Aulas.Update(aula);
            await _context.SaveChangesAsync();

            return aula;
        }

        // ============================================================
        // 7. DELETAR AULA
        // ============================================================

        /// <summary>
        /// Remove uma aula do banco de dados.
        ///
        /// No fluxo atual, a aula é removida fisicamente quando solicitada.
        /// Caso o registro não exista, retorna false para que o Controller responda
        /// com NotFound.
        /// </summary>
        public async Task<bool> Deletar(int id)
        {
            var aula = await _context.Aulas.FindAsync(id);

            if (aula == null)
                return false;

            _context.Aulas.Remove(aula);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}