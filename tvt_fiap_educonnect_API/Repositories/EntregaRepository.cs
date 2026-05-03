using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade EntregaAtividade.
    ///
    /// No EduConnect, a entrega representa a submissão de uma atividade realizada
    /// por um aluno. Essa camada centraliza criação, consulta, listagem por atividade,
    /// listagem por aluno e atualização da entrega.
    ///
    /// As consultas carregam relacionamentos como Aluno, Usuario, Atividade
    /// e Disciplina para permitir que o Service monte DTOs completos para o frontend.
    /// </summary>
    public class EntregaRepository : IEntregaRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext expõe a tabela de entregas e seus relacionamentos
        /// com alunos, usuários, atividades e disciplinas.
        /// </summary>
        public EntregaRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR ENTREGA
        // ============================================================

        /// <summary>
        /// Cria uma nova entrega de atividade no banco de dados.
        ///
        /// A entidade já chega montada pela camada de Service, que é responsável
        /// por validar o aluno, a atividade e preparar o arquivo enviado.
        /// </summary>
        public async Task<EntregaAtividade> Criar(EntregaAtividade entrega)
        {
            _context.EntregasAtividades.Add(entrega);
            await _context.SaveChangesAsync();

            return entrega;
        }

        // ============================================================
        // 2. OBTER ENTREGA POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma entrega específica pelo ID.
        ///
        /// A consulta carrega o aluno e o usuário vinculado para permitir exibir
        /// dados como nome e e-mail do aluno.
        ///
        /// Também carrega a atividade relacionada para permitir identificar
        /// a qual atividade a entrega pertence.
        /// </summary>
        public async Task<EntregaAtividade?> ObterPorId(int id)
        {
            return await _context.EntregasAtividades
                .Include(e => e.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(e => e.Atividade)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        // ============================================================
        // 3. LISTAR ENTREGAS POR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Lista todas as entregas vinculadas a uma atividade específica.
        ///
        /// Esse método é usado principalmente na visão do professor,
        /// permitindo acompanhar quais alunos entregaram determinada atividade
        /// e quais entregas ainda precisam ser avaliadas.
        /// </summary>
        public async Task<IEnumerable<EntregaAtividade>> ListarPorAtividade(int atividadeId)
        {
            return await _context.EntregasAtividades
                .Where(e => e.AtividadeId == atividadeId)
                .Include(e => e.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(e => e.Atividade)
                .ToListAsync();
        }

        // ============================================================
        // 4. ATUALIZAR ENTREGA
        // ============================================================

        /// <summary>
        /// Atualiza uma entrega existente.
        ///
        /// Esse método é usado em fluxos como correção da atividade,
        /// atribuição de nota e registro de feedback do professor.
        /// </summary>
        public async Task<EntregaAtividade> Atualizar(EntregaAtividade entrega)
        {
            _context.EntregasAtividades.Update(entrega);
            await _context.SaveChangesAsync();

            return entrega;
        }

        // ============================================================
        // 5. LISTAR ENTREGAS POR ALUNO
        // ============================================================

        /// <summary>
        /// Lista todas as entregas realizadas por um aluno específico.
        ///
        /// Esse método é usado na visão do aluno para consultar seu histórico
        /// de entregas, notas e feedbacks.
        ///
        /// A consulta também carrega a atividade, a TurmaDisciplina e a Disciplina
        /// para permitir exibir o nome da disciplina relacionada à entrega.
        /// </summary>
        public async Task<IEnumerable<EntregaAtividade>> ListarPorAluno(int alunoId)
        {
            return await _context.EntregasAtividades
                .Where(e => e.AlunoId == alunoId)
                .Include(e => e.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(e => e.Atividade)
                    .ThenInclude(a => a.TurmaDisciplina)
                        .ThenInclude(td => td.Disciplina)
                .ToListAsync();
        }
    }
}