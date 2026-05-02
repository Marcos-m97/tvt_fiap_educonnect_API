using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas às turmas.
    ///
    /// No EduConnect, a turma pertence a um curso e representa uma oferta acadêmica
    /// em determinado período e semestre.
    ///
    /// Essa camada valida a existência do curso, cria turmas, lista registros,
    /// atualiza dados, aplica soft delete/reativação e converte entidades em DTOs.
    /// </summary>
    public class TurmaService : ITurmaService
    {
        private readonly ITurmaRepository _repo;
        private readonly ICursoRepository _cursoRepo;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// ITurmaRepository: acesso aos dados de turmas.
        /// ICursoRepository: usado para validar se o curso informado existe.
        /// </summary>
        public TurmaService(ITurmaRepository repo, ICursoRepository cursoRepo)
        {
            _repo = repo;
            _cursoRepo = cursoRepo;
        }

        // ============================================================
        // 1. CRIAR TURMA
        // ============================================================

        /// <summary>
        /// Cria uma nova turma vinculada a um curso existente.
        ///
        /// Antes de salvar, o sistema valida se o CursoId informado corresponde
        /// a um curso cadastrado. Isso evita criar turmas órfãs sem curso.
        /// </summary>
        public async Task<TurmaDTO> Criar(CriarTurmaDTO dto)
        {
            var curso = await _cursoRepo.ObterPorId(dto.CursoId);

            if (curso == null)
                throw new AppException("Curso não encontrado.", 404);

            var turma = new Turma
            {
                Nome = dto.Nome,
                Periodo = dto.Periodo,
                Semestre = dto.Semestre,
                CursoId = dto.CursoId,
                Ativo = true
            };

            turma = await _repo.Criar(turma);

            return MapToDTO(turma);
        }

        // ============================================================
        // 2. OBTER TURMA POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma turma pelo ID.
        ///
        /// Caso a turma não exista, retorna null para que o Controller
        /// responda com NotFound.
        /// </summary>
        public async Task<TurmaDTO?> ObterPorId(int id)
        {
            var turma = await _repo.ObterPorId(id);

            return turma == null ? null : MapToDTO(turma);
        }

        // ============================================================
        // 3. LISTAR TURMAS
        // ============================================================

        /// <summary>
        /// Lista todas as turmas cadastradas.
        ///
        /// As entidades retornadas pelo Repository são convertidas para DTO
        /// antes de serem enviadas ao frontend.
        /// </summary>
        public async Task<IEnumerable<TurmaDTO>> Listar()
        {
            var list = await _repo.Listar();

            return list.Select(MapToDTO);
        }

        // ============================================================
        // 4. LISTAR TURMAS POR CURSO
        // ============================================================

        /// <summary>
        /// Lista turmas vinculadas a um curso específico.
        ///
        /// Esse método é usado para filtrar as turmas disponíveis de acordo
        /// com o curso selecionado no frontend.
        /// </summary>
        public async Task<IEnumerable<TurmaDTO>> ListarPorCurso(int cursoId)
        {
            var list = await _repo.ListarPorCurso(cursoId);

            return list.Select(MapToDTO);
        }

        // ============================================================
        // 5. ATUALIZAR TURMA
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma turma existente.
        ///
        /// O Service busca a entidade no banco, valida se o novo CursoId existe,
        /// altera os campos permitidos e delega a persistência ao Repository.
        /// </summary>
        public async Task<TurmaDTO?> Atualizar(int id, CriarTurmaDTO dto)
        {
            var turma = await _repo.ObterPorId(id);

            if (turma == null)
                return null;

            var curso = await _cursoRepo.ObterPorId(dto.CursoId);

            if (curso == null)
                throw new AppException("Curso não encontrado.", 404);

            turma.Nome = dto.Nome;
            turma.Periodo = dto.Periodo;
            turma.Semestre = dto.Semestre;
            turma.CursoId = dto.CursoId;

            turma = await _repo.Atualizar(turma);

            return MapToDTO(turma);
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente uma turma.
        ///
        /// A operação é delegada ao Repository, que altera o campo Ativo para false.
        /// </summary>
        public Task<bool> Deletar(int id)
        {
            return _repo.Deletar(id);
        }

        // ============================================================
        // 7. REATIVAR TURMA
        // ============================================================

        /// <summary>
        /// Reativa uma turma previamente desativada.
        ///
        /// A operação é delegada ao Repository, que altera o campo Ativo para true.
        /// </summary>
        public Task<bool> Reativar(int id)
        {
            return _repo.Reativar(id);
        }

        // ============================================================
        // 8. MAPEAMENTO PARA DTO
        // ============================================================

        /// <summary>
        /// Converte a entidade Turma em TurmaDTO.
        ///
        /// O DTO inclui dados próprios da turma e também o nome do curso,
        /// quando a relação foi carregada pelo Repository com Include.
        /// </summary>
        private TurmaDTO MapToDTO(Turma t)
        {
            return new TurmaDTO
            {
                Id = t.Id,
                Nome = t.Nome,
                Periodo = t.Periodo,
                Semestre = t.Semestre,
                CursoId = t.CursoId,
                CursoNome = t.Curso?.Nome ?? string.Empty,
                Ativo = t.Ativo
            };
        }
    }
}