using EduConnect_API.Exceptions;
using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas às disciplinas.
    ///
    /// No EduConnect, a disciplina pertence a um curso e compõe a estrutura acadêmica
    /// que posteriormente será associada a turmas, professores, aulas e atividades.
    ///
    /// Essa camada valida a existência do curso, cria disciplinas, lista registros,
    /// atualiza dados, aplica soft delete/reativação e converte entidades em DTOs.
    /// </summary>
    public class DisciplinaService : IDisciplinaService
    {
        private readonly IDisciplinaRepository _repo;
        private readonly ICursoRepository _cursoRepo;

        /// <summary>
        /// Recebe as dependências por injeção de dependência.
        ///
        /// IDisciplinaRepository: acesso aos dados de disciplinas.
        /// ICursoRepository: usado para validar se o curso informado existe.
        /// </summary>
        public DisciplinaService(IDisciplinaRepository repo, ICursoRepository cursoRepo)
        {
            _repo = repo;
            _cursoRepo = cursoRepo;
        }

        // ============================================================
        // 1. CRIAR DISCIPLINA
        // ============================================================

        /// <summary>
        /// Cria uma nova disciplina vinculada a um curso existente.
        ///
        /// Antes de salvar, o sistema valida se o CursoId informado corresponde
        /// a um curso cadastrado. Isso evita criar disciplinas órfãs sem curso.
        /// </summary>
        public async Task<DisciplinaDTO> Criar(CriarDisciplinaDTO dto)
        {
            var curso = await _cursoRepo.ObterPorId(dto.CursoId);

            if (curso == null)
                throw new AppException("Curso não encontrado.", 404);

            var disciplina = new Disciplina
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                CargaHoraria = dto.CargaHoraria,
                CursoId = dto.CursoId,
                Ativo = true
            };

            disciplina = await _repo.Criar(disciplina);

            return MapToDTO(disciplina);
        }

        // ============================================================
        // 2. LISTAR DISCIPLINAS
        // ============================================================

        /// <summary>
        /// Lista todas as disciplinas cadastradas.
        ///
        /// As entidades retornadas pelo Repository são convertidas para DTO
        /// antes de serem enviadas ao frontend.
        /// </summary>
        public async Task<IEnumerable<DisciplinaDTO>> Listar()
        {
            var list = await _repo.Listar();

            return list.Select(MapToDTO);
        }

        // ============================================================
        // 3. LISTAR DISCIPLINAS POR CURSO
        // ============================================================

        /// <summary>
        /// Lista disciplinas vinculadas a um curso específico.
        ///
        /// Esse método é usado para filtrar a grade acadêmica conforme
        /// o curso selecionado no frontend.
        /// </summary>
        public async Task<IEnumerable<DisciplinaDTO>> ListarPorCurso(int cursoId)
        {
            var list = await _repo.ListarPorCurso(cursoId);

            return list.Select(MapToDTO);
        }

        // ============================================================
        // 4. OBTER DISCIPLINA POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma disciplina pelo ID.
        ///
        /// Caso a disciplina não exista, retorna null para que o Controller
        /// responda com NotFound.
        /// </summary>
        public async Task<DisciplinaDTO?> ObterPorId(int id)
        {
            var disciplina = await _repo.ObterPorId(id);

            return disciplina == null ? null : MapToDTO(disciplina);
        }

        // ============================================================
        // 5. ATUALIZAR DISCIPLINA
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma disciplina existente.
        ///
        /// O Service busca a entidade no banco, altera os campos permitidos
        /// e delega a persistência ao Repository.
        ///
        /// Uma melhoria possível seria validar também se o novo CursoId informado
        /// existe antes de alterar o vínculo da disciplina.
        /// </summary>
        public async Task<DisciplinaDTO?> Atualizar(int id, CriarDisciplinaDTO dto)
        {
            var disciplina = await _repo.ObterPorId(id);

            if (disciplina == null)
                return null;

            disciplina.Nome = dto.Nome;
            disciplina.Descricao = dto.Descricao;
            disciplina.CargaHoraria = dto.CargaHoraria;
            disciplina.CursoId = dto.CursoId;

            disciplina = await _repo.Atualizar(disciplina);

            return MapToDTO(disciplina);
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente uma disciplina.
        ///
        /// A operação é delegada ao Repository, que altera o campo Ativo para false.
        /// </summary>
        public async Task<bool> Deletar(int id)
        {
            return await _repo.Deletar(id);
        }

        // ============================================================
        // 7. REATIVAR DISCIPLINA
        // ============================================================

        /// <summary>
        /// Reativa uma disciplina previamente desativada.
        ///
        /// O Service busca a disciplina, altera o campo Ativo para true
        /// e reutiliza o método Atualizar do Repository para persistir a alteração.
        /// </summary>
        public async Task<bool> Reativar(int id)
        {
            var disciplina = await _repo.ObterPorId(id);

            if (disciplina == null)
                return false;

            disciplina.Ativo = true;

            await _repo.Atualizar(disciplina);

            return true;
        }

        // ============================================================
        // 8. MAPEAMENTO PARA DTO
        // ============================================================

        /// <summary>
        /// Converte a entidade Disciplina em DisciplinaDTO.
        ///
        /// O DTO inclui dados próprios da disciplina e também o nome do curso,
        /// quando a relação foi carregada pelo Repository com Include.
        /// </summary>
        private DisciplinaDTO MapToDTO(Disciplina d)
        {
            return new DisciplinaDTO
            {
                Id = d.Id,
                Nome = d.Nome,
                Descricao = d.Descricao,
                CargaHoraria = d.CargaHoraria,
                CursoId = d.CursoId,
                CursoNome = d.Curso?.Nome ?? string.Empty,
                Ativo = d.Ativo
            };
        }
    }
}