using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using tvt_fiap_educonnect_API.Models.DTOs;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas
    /// ao vínculo entre turma, disciplina e professor.
    ///
    /// No EduConnect, essa camada garante que a turma, a disciplina e o professor
    /// existam antes de criar ou atualizar uma associação acadêmica.
    ///
    /// Também centraliza o mapeamento da entidade para DTO, retornando ao frontend
    /// nomes descritivos como turma, disciplina e professor.
    /// </summary>
    public class TurmaDisciplinaService : ITurmaDisciplinaService
    {
        private readonly ITurmaDisciplinaRepository _repo;
        private readonly ITurmaRepository _turmaRepo;
        private readonly IDisciplinaRepository _disciplinaRepo;
        private readonly IProfessorRepository _professorRepo;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// Cada repositório é utilizado para validar uma parte da associação:
        /// turma, disciplina e professor.
        /// </summary>
        public TurmaDisciplinaService(
            ITurmaDisciplinaRepository repo,
            ITurmaRepository turmaRepo,
            IDisciplinaRepository disciplinaRepo,
            IProfessorRepository professorRepo)
        {
            _repo = repo;
            _turmaRepo = turmaRepo;
            _disciplinaRepo = disciplinaRepo;
            _professorRepo = professorRepo;
        }

        // ============================================================
        // 1. CRIAR VÍNCULO
        // ============================================================

        /// <summary>
        /// Cria uma nova associação entre turma, disciplina e professor.
        ///
        /// Antes de salvar, o sistema valida se as três entidades existem.
        /// Isso evita vínculos acadêmicos inválidos ou órfãos.
        /// </summary>
        public async Task<TurmaDisciplinaDTO> Criar(CriarTurmaDisciplinaDTO dto)
        {
            var turma = await _turmaRepo.ObterPorId(dto.TurmaId)
                ?? throw new AppException("Turma não encontrada.", 404);

            var disciplina = await _disciplinaRepo.ObterPorId(dto.DisciplinaId)
                ?? throw new AppException("Disciplina não encontrada.", 404);

            var professor = await _professorRepo.ObterPorId(dto.ProfessorId)
                ?? throw new AppException("Professor não encontrado.", 404);

            var entity = new TurmaDisciplina
            {
                TurmaId = dto.TurmaId,
                DisciplinaId = dto.DisciplinaId,
                ProfessorId = dto.ProfessorId
            };

            entity = await _repo.Criar(entity);

            return MapToDTO(
                entity,
                turma.Nome,
                disciplina.Nome,
                professor.Usuario.Nome
            );
        }

        // ============================================================
        // 2. OBTER POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma associação específica pelo ID.
        ///
        /// O Repository já carrega os relacionamentos necessários para montar
        /// um DTO com nomes de turma, disciplina e professor.
        /// </summary>
        public async Task<TurmaDisciplinaDTO?> ObterPorId(int id)
        {
            var entity = await _repo.ObterPorId(id);

            if (entity == null)
                return null;

            return MapToDTO(
                entity,
                entity.Turma.Nome,
                entity.Disciplina.Nome,
                entity.Professor.Usuario.Nome
            );
        }

        // ============================================================
        // 3. LISTAR TODOS
        // ============================================================

        /// <summary>
        /// Lista todas as associações entre turmas, disciplinas e professores.
        ///
        /// Cada entidade é convertida para DTO antes do retorno ao frontend.
        /// </summary>
        public async Task<IEnumerable<TurmaDisciplinaDTO>> Listar()
        {
            var list = await _repo.Listar();

            return list.Select(e =>
                MapToDTO(
                    e,
                    e.Turma.Nome,
                    e.Disciplina.Nome,
                    e.Professor.Usuario.Nome
                )
            );
        }

        // ============================================================
        // 4. LISTAR POR TURMA
        // ============================================================

        /// <summary>
        /// Lista as disciplinas e professores associados a uma turma específica.
        ///
        /// Esse método é usado para montar a grade acadêmica da turma.
        /// </summary>
        public async Task<IEnumerable<TurmaDisciplinaDTO>> ListarPorTurma(int turmaId)
        {
            var list = await _repo.ListarPorTurma(turmaId);

            return list.Select(e =>
                MapToDTO(
                    e,
                    e.Turma.Nome,
                    e.Disciplina.Nome,
                    e.Professor.Usuario.Nome
                )
            );
        }

        // ============================================================
        // 5. ATUALIZAR VÍNCULO
        // ============================================================

        /// <summary>
        /// Atualiza uma associação entre turma, disciplina e professor.
        ///
        /// Antes de salvar, o sistema valida se a nova turma, disciplina e professor
        /// informados existem. Isso evita que a associação seja atualizada com
        /// chaves estrangeiras inválidas.
        /// </summary>
        public async Task<TurmaDisciplinaDTO?> Atualizar(int id, CriarTurmaDisciplinaDTO dto)
        {
            var entity = await _repo.ObterPorId(id);

            if (entity == null)
                return null;

            var turma = await _turmaRepo.ObterPorId(dto.TurmaId)
                ?? throw new AppException("Turma não encontrada.", 404);

            var disciplina = await _disciplinaRepo.ObterPorId(dto.DisciplinaId)
                ?? throw new AppException("Disciplina não encontrada.", 404);

            var professor = await _professorRepo.ObterPorId(dto.ProfessorId)
                ?? throw new AppException("Professor não encontrado.", 404);

            entity.TurmaId = dto.TurmaId;
            entity.DisciplinaId = dto.DisciplinaId;
            entity.ProfessorId = dto.ProfessorId;

            entity = await _repo.Atualizar(entity);

            return MapToDTO(
                entity,
                turma.Nome,
                disciplina.Nome,
                professor.Usuario.Nome
            );
        }

        // ============================================================
        // 6. DELETAR VÍNCULO
        // ============================================================

        /// <summary>
        /// Remove uma associação entre turma, disciplina e professor.
        ///
        /// A operação é delegada ao Repository.
        /// </summary>
        public Task<bool> Deletar(int id)
        {
            return _repo.Deletar(id);
        }

        // ============================================================
        // 7. MAPEAMENTO PARA DTO
        // ============================================================

        /// <summary>
        /// Converte a entidade TurmaDisciplina em TurmaDisciplinaDTO.
        ///
        /// O DTO combina os IDs técnicos com nomes descritivos, facilitando
        /// a exibição no frontend sem exigir novas consultas.
        /// </summary>
        private TurmaDisciplinaDTO MapToDTO(
            TurmaDisciplina e,
            string turmaNome,
            string disciplinaNome,
            string professorNome)
        {
            return new TurmaDisciplinaDTO
            {
                Id = e.Id,
                TurmaId = e.TurmaId,
                TurmaNome = turmaNome,
                DisciplinaId = e.DisciplinaId,
                DisciplinaNome = disciplinaNome,
                ProfessorId = e.ProfessorId,
                ProfessorNome = professorNome
            };
        }
    }
}