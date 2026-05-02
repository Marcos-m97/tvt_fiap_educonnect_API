using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados à associação entre turma,
    /// disciplina e professor.
    ///
    /// No EduConnect, a entidade TurmaDisciplina define quais disciplinas serão
    /// ofertadas em uma turma específica e qual professor será responsável por elas.
    ///
    /// Essa camada recebe as requisições HTTP do frontend e delega as regras
    /// de negócio para o TurmaDisciplinaService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TurmaDisciplinaController : ControllerBase
    {
        private readonly ITurmaDisciplinaService _service;

        /// <summary>
        /// Recebe o serviço de TurmaDisciplina por injeção de dependência.
        /// </summary>
        public TurmaDisciplinaController(ITurmaDisciplinaService service)
        {
            _service = service;
        }

        // ============================================================
        // 1. CRIAR VÍNCULO TURMA-DISCIPLINA-PROFESSOR
        // ============================================================

        /// <summary>
        /// Cria uma associação entre turma, disciplina e professor.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois altera a estrutura
        /// acadêmica da plataforma.
        ///
        /// A regra de negócio principal fica no Service, que valida se a turma,
        /// a disciplina e o professor informados existem antes de criar o vínculo.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarTurmaDisciplinaDTO dto)
        {
            var result = await _service.Criar(dto);

            return Ok(result);
        }

        // ============================================================
        // 2. LISTAR TODOS OS VÍNCULOS
        // ============================================================

        /// <summary>
        /// Lista todos os vínculos entre turmas, disciplinas e professores.
        ///
        /// Esse endpoint pode ser acessado por todos os perfis autenticados,
        /// pois essas informações são usadas em diferentes áreas do sistema:
        /// administração, professor e aluno.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var result = await _service.Listar();

            return Ok(result);
        }

        // ============================================================
        // 3. OBTER VÍNCULO POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma associação específica pelo ID.
        ///
        /// Esse endpoint retorna o vínculo com os dados relacionados de turma,
        /// disciplina e professor.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var result = await _service.ObterPorId(id);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ============================================================
        // 4. LISTAR VÍNCULOS POR TURMA
        // ============================================================

        /// <summary>
        /// Lista todas as disciplinas e professores associados a uma turma.
        ///
        /// Esse endpoint é usado para montar a grade acadêmica de uma turma,
        /// permitindo visualizar quais disciplinas serão ofertadas e quem será
        /// o professor responsável por cada uma.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("turma/{turmaId}")]
        public async Task<IActionResult> ListarPorTurma(int turmaId)
        {
            var result = await _service.ListarPorTurma(turmaId);

            return Ok(result);
        }

        // ============================================================
        // 5. ATUALIZAR VÍNCULO
        // ============================================================

        /// <summary>
        /// Atualiza uma associação entre turma, disciplina e professor.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois mudanças nesse vínculo
        /// podem impactar aulas, atividades e a visualização acadêmica dos alunos.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarTurmaDisciplinaDTO dto)
        {
            var result = await _service.Atualizar(id, dto);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // ============================================================
        // 6. DELETAR VÍNCULO
        // ============================================================

        /// <summary>
        /// Remove uma associação entre turma, disciplina e professor.
        ///
        /// Diferente de entidades principais como Curso, Turma e Disciplina,
        /// essa entidade representa um vínculo acadêmico. Por isso, neste fluxo
        /// o registro é removido fisicamente quando a associação deixa de existir.
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
    }
}