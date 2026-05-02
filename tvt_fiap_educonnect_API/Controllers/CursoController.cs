using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados aos cursos.
    ///
    /// No EduConnect, o curso representa uma estrutura acadêmica principal,
    /// usada para organizar disciplinas, turmas e o processo de matrícula.
    ///
    /// Essa camada recebe as requisições HTTP do frontend e delega as regras
    /// de negócio para o CursoService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CursoController : ControllerBase
    {
        private readonly ICursoService _service;

        /// <summary>
        /// Recebe o serviço de cursos por injeção de dependência.
        /// </summary>
        public CursoController(ICursoService service)
        {
            _service = service;
        }

        // ============================================================
        // 1. CRIAR CURSO
        // ============================================================

        /// <summary>
        /// Cria um novo curso.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois a criação de cursos
        /// faz parte da gestão acadêmica da plataforma.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarCursoDTO dto)
        {
            var curso = await _service.Criar(dto);

            return Ok(curso);
        }

        // ============================================================
        // 2. LISTAR CURSOS
        // ============================================================

        /// <summary>
        /// Lista cursos com paginação e busca opcional.
        ///
        /// Esse endpoint pode ser acessado por todos os perfis autenticados,
        /// pois cursos podem ser consultados em diferentes áreas do sistema,
        /// como matrícula, gestão acadêmica e visualização do aluno.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            var cursos = await _service.Listar(page, pageSize, search);

            return Ok(cursos);
        }

        // ============================================================
        // 3. OBTER CURSO POR ID
        // ============================================================

        /// <summary>
        /// Obtém os dados de um curso específico pelo ID.
        ///
        /// Esse endpoint é usado em telas de detalhe, edição ou seleção de curso.
        /// Caso o curso não exista, retorna NotFound.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var curso = await _service.ObterPorId(id);

            if (curso == null)
                return NotFound();

            return Ok(curso);
        }

        // ============================================================
        // 4. ATUALIZAR CURSO
        // ============================================================

        /// <summary>
        /// Atualiza os dados de um curso existente.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois altera a estrutura
        /// acadêmica do sistema.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarCursoDTO dto)
        {
            var curso = await _service.Atualizar(id, dto);

            if (curso == null)
                return NotFound();

            return Ok(curso);
        }

        // ============================================================
        // 5. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente um curso.
        ///
        /// Em vez de excluir o curso do banco, o sistema altera seu status para inativo.
        /// Isso preserva histórico de turmas, disciplinas e matrículas vinculadas.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var sucesso = await _service.Deletar(id);

            if (!sucesso)
                return NotFound();

            return NoContent();
        }

        // ============================================================
        // 6. REATIVAR CURSO
        // ============================================================

        /// <summary>
        /// Reativa um curso previamente desativado.
        ///
        /// Esse endpoint permite que um curso volte a ser utilizado no sistema
        /// sem necessidade de recriar seu cadastro.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("reativar/{id}")]
        public async Task<IActionResult> Reativar(int id)
        {
            var sucesso = await _service.Reativar(id);

            if (!sucesso)
                return NotFound();

            return NoContent();
        }
    }
}