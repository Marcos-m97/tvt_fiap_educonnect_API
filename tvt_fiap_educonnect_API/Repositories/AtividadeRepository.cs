using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Atividade.
    ///
    /// No EduConnect, a atividade representa uma tarefa, exercício, prova ou trabalho
    /// criado dentro de uma TurmaDisciplina.
    ///
    /// Essa camada centraliza criação, consulta, listagem por turma/disciplina,
    /// listagem por turma e atualização das atividades.
    /// </summary>
    public class AtividadeRepository : IAtividadeRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext expõe a tabela de atividades e seus relacionamentos
        /// com TurmaDisciplina, Turma, Disciplina, Professor e Entregas.
        /// </summary>
        public AtividadeRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Cria uma nova atividade no banco de dados.
        ///
        /// A entidade já chega montada pela camada de Service, que é responsável
        /// por validar o contexto acadêmico e preparar os dados antes da persistência.
        /// </summary>
        public async Task<Atividade> Criar(Atividade atividade)
        {
            _context.Atividades.Add(atividade);
            await _context.SaveChangesAsync();

            return atividade;
        }

        // ============================================================
        // 2. OBTER ATIVIDADE POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma atividade pelo ID, carregando seu contexto acadêmico completo.
        ///
        /// Os Includes permitem acessar:
        /// - Turma vinculada;
        /// - Disciplina vinculada;
        /// - Professor responsável;
        /// - Usuário relacionado ao professor.
        ///
        /// Isso permite que a camada de Service monte um DTO mais completo
        /// para o frontend.
        /// </summary>
        public async Task<Atividade?> ObterPorId(int id)
        {
            return await _context.Atividades
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Turma)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Professor)
                        .ThenInclude(p => p.Usuario)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        // ============================================================
        // 3. LISTAR ATIVIDADES POR TURMA/DISCIPLINA
        // ============================================================

        /// <summary>
        /// Lista as atividades vinculadas a uma TurmaDisciplina específica.
        ///
        /// Esse método é usado para exibir atividades de uma disciplina dentro
        /// de uma turma, normalmente na visão do professor ou do aluno.
        ///
        /// A consulta também carrega turma, disciplina e professor para permitir
        /// retorno de dados descritivos ao frontend.
        /// </summary>
        public async Task<IEnumerable<Atividade>> ListarPorTurmaDisciplina(int turmaDisciplinaId)
        {
            return await _context.Atividades
                .Where(a => a.TurmaDisciplinaId == turmaDisciplinaId)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Turma)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Professor)
                        .ThenInclude(p => p.Usuario)
                .ToListAsync();
        }

        // ============================================================
        // 4. LISTAR ATIVIDADES POR TURMA
        // ============================================================

        /// <summary>
        /// Lista todas as atividades disponíveis para uma turma.
        ///
        /// Esse método é útil na visão do aluno, pois após a matrícula efetivada
        /// o aluno acessa as atividades relacionadas à sua turma.
        ///
        /// A consulta filtra pelo ID da turma através da relação TurmaDisciplina
        /// e também carrega as entregas para permitir identificar submissões
        /// relacionadas às atividades.
        /// </summary>
        public async Task<IEnumerable<Atividade>> ListarPorTurma(int turmaId)
        {
            return await _context.Atividades
                .Where(a => a.TurmaDisciplina.TurmaId == turmaId)
                .Include(a => a.TurmaDisciplina)
                    .ThenInclude(td => td.Disciplina)
                .Include(a => a.Entregas)
                .ToListAsync();
        }

        // ============================================================
        // 5. ATUALIZAR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma atividade existente.
        ///
        /// A entidade já chega modificada pela camada de Service.
        /// </summary>
        public async Task<Atividade> Atualizar(Atividade atividade)
        {
            _context.Atividades.Update(atividade);
            await _context.SaveChangesAsync();

            return atividade;
        }
    }
}