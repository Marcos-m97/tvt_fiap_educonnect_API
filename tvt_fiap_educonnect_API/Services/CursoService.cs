using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por concentrar as regras de negócio relacionadas aos cursos.
    ///
    /// No EduConnect, o curso é uma estrutura acadêmica principal, usada como base
    /// para organizar disciplinas e turmas.
    ///
    /// Essa camada transforma DTOs em entidades, aplica paginação e busca,
    /// controla atualização e reativação, além de converter entidades em DTOs
    /// antes de retornar dados ao frontend.
    /// </summary>
    public class CursoService : ICursoService
    {
        private readonly ICursoRepository _repo;

        /// <summary>
        /// Recebe o repositório de cursos por injeção de dependência.
        /// </summary>
        public CursoService(ICursoRepository repo)
        {
            _repo = repo;
        }

        // ============================================================
        // 1. CRIAR CURSO
        // ============================================================

        /// <summary>
        /// Cria um novo curso no sistema.
        ///
        /// O DTO recebido do frontend é convertido para a entidade Curso,
        /// definindo o curso como ativo por padrão.
        /// </summary>
        public async Task<CursoDTO> Criar(CriarCursoDTO dto)
        {
            var curso = new Curso
            {
                Nome = dto.Nome,
                Descricao = dto.Descricao,
                CargaHoraria = dto.CargaHoraria,
                Ativo = true
            };

            curso = await _repo.Criar(curso);

            return MapToDTO(curso);
        }

        // ============================================================
        // 2. LISTAR CURSOS COM PAGINAÇÃO E BUSCA
        // ============================================================

        /// <summary>
        /// Lista cursos com paginação e busca opcional.
        ///
        /// A consulta é construída a partir de IQueryable, permitindo que filtros,
        /// ordenação e paginação sejam aplicados no banco de dados antes da execução.
        ///
        /// O retorno usa PagedResultDTO para entregar ao frontend os dados da página
        /// atual e o total de registros encontrados.
        /// </summary>
        public async Task<PagedResultDTO<CursoDTO>> Listar(
            int page,
            int pageSize,
            string? search)
        {
            var query = _repo.Query();

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(c => c.Nome.Contains(search));
            }

            var total = await query.CountAsync();

            var cursos = await query
                .OrderBy(c => c.Nome)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResultDTO<CursoDTO>
            {
                Data = cursos.Select(MapToDTO),
                Total = total
            };
        }

        // ============================================================
        // 3. OBTER CURSO POR ID
        // ============================================================

        /// <summary>
        /// Obtém os dados de um curso pelo ID.
        ///
        /// Caso o curso não exista, retorna null para que o Controller trate
        /// a resposta como NotFound.
        /// </summary>
        public async Task<CursoDTO?> ObterPorId(int id)
        {
            var curso = await _repo.ObterPorId(id);

            return curso == null ? null : MapToDTO(curso);
        }

        // ============================================================
        // 4. ATUALIZAR CURSO
        // ============================================================

        /// <summary>
        /// Atualiza os dados de um curso existente.
        ///
        /// O Service busca a entidade no banco, altera os campos permitidos
        /// e delega a persistência ao Repository.
        /// </summary>
        public async Task<CursoDTO?> Atualizar(int id, CriarCursoDTO dto)
        {
            var curso = await _repo.ObterPorId(id);

            if (curso == null)
                return null;

            curso.Nome = dto.Nome;
            curso.Descricao = dto.Descricao;
            curso.CargaHoraria = dto.CargaHoraria;

            var atualizado = await _repo.Atualizar(curso);

            return MapToDTO(atualizado);
        }

        // ============================================================
        // 5. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente um curso.
        ///
        /// A operação é delegada ao Repository, que altera o campo Ativo para false.
        /// </summary>
        public Task<bool> Deletar(int id)
        {
            return _repo.Deletar(id);
        }

        // ============================================================
        // 6. REATIVAR CURSO
        // ============================================================

        /// <summary>
        /// Reativa um curso previamente desativado.
        ///
        /// A operação é delegada ao Repository, que altera o campo Ativo para true.
        /// </summary>
        public Task<bool> Reativar(int id)
        {
            return _repo.Reativar(id);
        }

        // ============================================================
        // 7. MAPEAMENTO PARA DTO
        // ============================================================

        /// <summary>
        /// Converte a entidade Curso em CursoDTO.
        ///
        /// Esse mapeamento evita expor diretamente a entidade do banco ao frontend
        /// e centraliza quais dados de curso são retornados pela API.
        /// </summary>
        private CursoDTO MapToDTO(Curso c)
        {
            return new CursoDTO
            {
                Id = c.Id,
                Nome = c.Nome,
                Descricao = c.Descricao,
                CargaHoraria = c.CargaHoraria,
                Ativo = c.Ativo
            };
        }
    }
}