using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados às turmas.
    ///
    /// No EduConnect, a turma representa uma oferta de um curso em um determinado
    /// período e semestre. Ela é a estrutura que permite organizar alunos,
    /// disciplinas e professores dentro de um contexto acadêmico específico.
    ///
    /// Essa camada recebe as requisições HTTP do frontend e delega as regras
    /// de negócio para o TurmaService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TurmaController : ControllerBase
    {
        private readonly ITurmaService _service;

        /// <summary>
        /// Recebe o serviço de turmas por injeção de dependência.
        /// </summary>
        public TurmaController(ITurmaService service)
        {
            _service = service;
        }

        // ============================================================
        // 1. CRIAR TURMA
        // ============================================================

        /// <summary>
        /// Cria uma nova turma vinculada a um curso.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois a criação de turmas
        /// faz parte da gestão acadêmica da plataforma.
        ///
        /// A regra de negócio principal fica no TurmaService, que valida se o curso
        /// informado existe antes de criar a turma.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarTurmaDTO dto)
        {
            var turma = await _service.Criar(dto);

            return Ok(turma);
        }

        // ============================================================
        // 2. LISTAR TURMAS
        // ============================================================

        /// <summary>
        /// Lista todas as turmas cadastradas.
        ///
        /// Esse endpoint pode ser acessado por todos os perfis autenticados,
        /// pois turmas são consultadas em diferentes fluxos do sistema,
        /// como matrícula, gestão acadêmica, visão do professor e visão do aluno.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var turmas = await _service.Listar();

            return Ok(turmas);
        }

        // ============================================================
        // 3. OBTER TURMA POR ID
        // ============================================================

        /// <summary>
        /// Obtém os dados de uma turma específica pelo ID.
        ///
        /// Esse endpoint é usado em telas de detalhe, edição ou seleção de turma.
        /// Caso a turma não exista, retorna NotFound.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var turma = await _service.ObterPorId(id);

            if (turma == null)
                return NotFound();

            return Ok(turma);
        }

        // ============================================================
        // 4. LISTAR TURMAS POR CURSO
        // ============================================================

        /// <summary>
        /// Lista turmas vinculadas a um curso específico.
        ///
        /// Esse endpoint é utilizado quando o frontend precisa filtrar as turmas
        /// com base no curso selecionado, por exemplo no fluxo de matrícula
        /// ou na gestão acadêmica.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("curso/{cursoId}")]
        public async Task<IActionResult> ListarPorCurso(int cursoId)
        {
            var turmas = await _service.ListarPorCurso(cursoId);

            return Ok(turmas);
        }

        // ============================================================
        // 5. ATUALIZAR TURMA
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma turma existente.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois alterações em turmas
        /// impactam diretamente a estrutura acadêmica, matrículas e associações
        /// com disciplinas.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarTurmaDTO dto)
        {
            var turma = await _service.Atualizar(id, dto);

            if (turma == null)
                return NotFound();

            return Ok(turma);
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente uma turma.
        ///
        /// Em vez de excluir fisicamente a turma do banco, o sistema altera
        /// seu status para inativo.
        ///
        /// Essa abordagem preserva histórico de matrículas, disciplinas associadas
        /// e registros acadêmicos relacionados à turma.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var ok = await _service.Deletar(id);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        // ============================================================
        // 7. REATIVAR TURMA
        // ============================================================

        /// <summary>
        /// Reativa uma turma previamente desativada.
        ///
        /// Esse endpoint permite que uma turma volte a ser utilizada no sistema
        /// sem necessidade de recriar seu cadastro.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("reativar/{id}")]
        public async Task<IActionResult> Reativar(int id)
        {
            var ok = await _service.Reativar(id);

            if (!ok)
                return NotFound();

            return NoContent();
        }
    }
}