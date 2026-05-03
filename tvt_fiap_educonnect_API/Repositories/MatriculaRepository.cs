using EduConnect_API.Data;
using EduConnect_API.Models;
using EduConnect_API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Repositories
{
    /// <summary>
    /// Repositório responsável pelas operações de banco de dados da entidade Matricula.
    ///
    /// No EduConnect, a matrícula representa o vínculo entre um aluno e uma turma,
    /// além de controlar o avanço do aluno no processo de entrada acadêmica.
    ///
    /// Essa camada centraliza consultas, persistência, listagens por aluno/turma,
    /// paginação e remoção de registros de matrícula.
    /// </summary>
    public class MatriculaRepository : IMatriculaRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Recebe o contexto do banco de dados por injeção de dependência.
        ///
        /// O AppDbContext expõe a tabela de matrículas e seus relacionamentos
        /// com aluno, usuário, turma e curso.
        /// </summary>
        public MatriculaRepository(AppDbContext context)
        {
            _context = context;
        }

        // ==============================================================
        // 1. CRIAR MATRÍCULA
        // ==============================================================

        /// <summary>
        /// Cria uma nova matrícula no banco de dados.
        ///
        /// A entidade já chega montada pela camada de Service, que é responsável
        /// por validar o aluno, a turma e o status inicial.
        /// </summary>
        public async Task<Matricula> Criar(Matricula matricula)
        {
            _context.Matriculas.Add(matricula);
            await _context.SaveChangesAsync();

            return matricula;
        }

        // ==============================================================
        // 2. OBTER MATRÍCULA POR ID
        // ==============================================================

        /// <summary>
        /// Obtém uma matrícula pelo ID, carregando os dados relacionados.
        ///
        /// O Include em Aluno e ThenInclude em Usuario permitem acessar nome,
        /// e-mail e UsuarioId do aluno.
        ///
        /// O Include em Turma permite retornar dados como nome da turma
        /// no DTO de matrícula.
        /// </summary>
        public async Task<Matricula?> ObterPorId(int id)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        // ==============================================================
        // 3. LISTAR TODAS AS MATRÍCULAS
        // ==============================================================

        /// <summary>
        /// Lista todas as matrículas cadastradas.
        ///
        /// Essa consulta carrega o aluno, o usuário do aluno e a turma para permitir
        /// que a camada de Service monte DTOs completos para o frontend.
        /// </summary>
        public async Task<IEnumerable<Matricula>> Listar()
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .ToListAsync();
        }

        // ==============================================================
        // 4. LISTAR MATRÍCULAS POR ALUNO
        // ==============================================================

        /// <summary>
        /// Lista todas as matrículas de um aluno específico.
        ///
        /// Esse método é usado principalmente na visão do aluno, permitindo
        /// acompanhar suas solicitações e seus respectivos status.
        /// </summary>
        public async Task<IEnumerable<Matricula>> ListarPorAluno(int alunoId)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .Where(m => m.AlunoId == alunoId)
                .ToListAsync();
        }

        // ==============================================================
        // 5. LISTAR MATRÍCULAS POR TURMA
        // ==============================================================

        /// <summary>
        /// Lista todas as matrículas vinculadas a uma turma específica.
        ///
        /// Esse método é mantido para consultas gerais por turma e também para
        /// preservar compatibilidade com a interface existente.
        /// </summary>
        public async Task<IEnumerable<Matricula>> ListarPorTurma(int turmaId)
        {
            return await _context.Matriculas
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .Where(m => m.TurmaId == turmaId)
                .ToListAsync();
        }

        // ==============================================================
        // 6. LISTAR ALUNOS EFETIVADOS POR TURMA COM PAGINAÇÃO
        // ==============================================================

        /// <summary>
        /// Lista alunos efetivados em uma turma com paginação e busca opcional.
        ///
        /// Diferente de ListarPorTurma, este método filtra apenas matrículas
        /// com status Efetivada, pois seu objetivo é montar a lista de alunos
        /// realmente ativos naquela turma.
        ///
        /// A busca opcional filtra pelo nome do aluno.
        /// </summary>
        public async Task<(IEnumerable<Matricula> Items, int TotalCount)>
            ListarAlunosPorTurmaPaginado(
                int turmaId,
                int page,
                int pageSize,
                string? search)
        {
            var query = _context.Matriculas
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .Where(m =>
                    m.TurmaId == turmaId &&
                    m.Status == MatriculaStatus.Efetivada)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.ToLower();

                query = query.Where(m =>
                    m.Aluno.Usuario.Nome.ToLower().Contains(search));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(m => m.Aluno.Usuario.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // ==============================================================
        // 7. ATUALIZAR MATRÍCULA
        // ==============================================================

        /// <summary>
        /// Atualiza os dados de uma matrícula existente.
        ///
        /// A entidade já chega modificada pela camada de Service, por exemplo
        /// com novo status, arquivo anexado ou data de atualização.
        /// </summary>
        public async Task<Matricula> Atualizar(Matricula matricula)
        {
            _context.Matriculas.Update(matricula);
            await _context.SaveChangesAsync();

            return matricula;
        }

        // ==============================================================
        // 8. DELETAR MATRÍCULA
        // ==============================================================

        /// <summary>
        /// Remove fisicamente uma matrícula do banco de dados.
        ///
        /// Diferente de entidades como Curso e Turma, este fluxo atual remove
        /// a matrícula diretamente quando solicitado pela administração.
        /// </summary>
        public async Task<bool> Deletar(int id)
        {
            var matricula = await _context.Matriculas.FindAsync(id);

            if (matricula == null)
                return false;

            _context.Matriculas.Remove(matricula);
            await _context.SaveChangesAsync();

            return true;
        }

        // ==============================================================
        // 9. OBTER MATRÍCULA ATIVA DO ALUNO
        // ==============================================================

        /// <summary>
        /// Obtém a matrícula efetivada de um aluno.
        ///
        /// Esse método é usado para identificar se o aluno já possui acesso acadêmico
        /// liberado. A consulta carrega também a turma, o curso, o aluno e o usuário.
        ///
        /// A matrícula é considerada ativa quando seu status está como Efetivada.
        /// </summary>
        public async Task<Matricula?> ObterAtivaPorAlunoId(int alunoId)
        {
            return await _context.Matriculas
                .Include(m => m.Turma)
                    .ThenInclude(t => t.Curso)
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .FirstOrDefaultAsync(m =>
                    m.AlunoId == alunoId &&
                    m.Status == MatriculaStatus.Efetivada);
        }

        // ==============================================================
        // 10. LISTAR MATRÍCULAS PAGINADO COM FILTROS
        // ==============================================================

        /// <summary>
        /// Lista matrículas com paginação, busca opcional por nome do aluno
        /// e filtro opcional por status.
        ///
        /// Esse método é utilizado no painel administrativo para acompanhar
        /// solicitações de matrícula sem carregar todos os registros em memória.
        ///
        /// O retorno inclui os itens da página atual e o total de registros
        /// encontrados para montagem da paginação no frontend.
        /// </summary>
        public async Task<(IEnumerable<Matricula> Items, int TotalCount)> ListarPaginado(
            int page,
            int pageSize,
            string? search,
            MatriculaStatus? status)
        {
            var query = _context.Matriculas
                .Include(m => m.Aluno)
                    .ThenInclude(a => a.Usuario)
                .Include(m => m.Turma)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(m =>
                    m.Aluno.Usuario.Nome.Contains(search));
            }

            if (status.HasValue)
            {
                query = query.Where(m => m.Status == status.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(m => m.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }
    }
}