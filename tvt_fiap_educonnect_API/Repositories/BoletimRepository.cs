using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Boletim.
    ///
    /// No EduConnect, o boletim consolida o desempenho acadêmico de um aluno
    /// em uma turma, agrupando os resultados por disciplina.
    ///
    /// Essa camada centraliza a criação do boletim e consultas por ID ou por aluno,
    /// utilizando Entity Framework para carregar as disciplinas relacionadas.
    /// </summary>
    public class BoletimRepository : IBoletimRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext expõe a tabela de boletins e seus relacionamentos
        /// com disciplinas e atividades do boletim.
        /// </summary>
        public BoletimRepository(AppDbContext context)
        {
            _context = context;
        }

        // ============================================================
        // 1. CRIAR BOLETIM
        // ============================================================

        /// <summary>
        /// Cria um novo boletim no banco de dados.
        ///
        /// A entidade já chega montada pela camada de Service, contendo
        /// as disciplinas e atividades consolidadas do aluno.
        /// </summary>
        public async Task<Boletim> Criar(Boletim boletim)
        {
            _context.Boletins.Add(boletim);
            await _context.SaveChangesAsync();

            return boletim;
        }

        // ============================================================
        // 2. OBTER BOLETIM POR ID
        // ============================================================

        /// <summary>
        /// Obtém um boletim específico pelo ID.
        ///
        /// A consulta carrega as disciplinas relacionadas ao boletim,
        /// permitindo exibir o desempenho do aluno agrupado por disciplina.
        ///
        /// Uma melhoria futura seria incluir também as atividades de cada disciplina,
        /// caso o frontend precise exibir o detalhe completo em uma única consulta.
        /// </summary>
        public async Task<Boletim?> Obter(int id)
        {
            return await _context.Boletins
                .Include(b => b.Disciplinas)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        // ============================================================
        // 3. LISTAR BOLETINS POR ALUNO
        // ============================================================

        /// <summary>
        /// Lista todos os boletins vinculados a um aluno específico.
        ///
        /// Esse método é usado na visão do aluno ou em consultas administrativas
        /// para acompanhar o histórico acadêmico consolidado do aluno.
        /// </summary>
        public async Task<IEnumerable<Boletim>> ListarPorAluno(int alunoId)
        {
            return await _context.Boletins
                .Include(b => b.Disciplinas)
                .Where(b => b.AlunoId == alunoId)
                .ToListAsync();
        }
    }
}