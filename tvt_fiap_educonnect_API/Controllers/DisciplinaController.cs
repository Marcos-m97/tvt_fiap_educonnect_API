using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados às disciplinas.
    ///
    /// No EduConnect, a disciplina representa uma unidade acadêmica vinculada
    /// a um curso. Ela pode posteriormente ser associada a turmas e professores
    /// por meio da entidade TurmaDisciplina.
    ///
    /// Essa camada recebe as requisições HTTP do frontend e delega as regras
    /// de negócio para o DisciplinaService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DisciplinaController : ControllerBase
    {
        private readonly IDisciplinaService _service;

        /// <summary>
        /// Recebe o serviço de disciplinas por injeção de dependência.
        /// </summary>
        public DisciplinaController(IDisciplinaService service)
        {
            _service = service;
        }

        // ============================================================
        // 1. CRIAR DISCIPLINA
        // ============================================================

        /// <summary>
        /// Cria uma nova disciplina.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois a criação
        /// de disciplinas faz parte da estruturação acadêmica do curso.
        ///
        /// A regra de negócio principal fica no DisciplinaService, que deve validar
        /// os dados recebidos e vincular a disciplina ao curso correspondente.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarDisciplinaDTO dto)
        {
            var result = await _service.Criar(dto);

            return Ok(result);
        }

        // ============================================================
        // 2. LISTAR DISCIPLINAS
        // ============================================================

        /// <summary>
        /// Lista todas as disciplinas cadastradas.
        ///
        /// Esse endpoint é acessível por perfis administrativos e professores,
        /// pois esses usuários podem precisar consultar disciplinas para gestão,
        /// associação com turmas ou acompanhamento acadêmico.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var disciplinas = await _service.Listar();

            return Ok(disciplinas);
        }

        // ============================================================
        // 3. LISTAR DISCIPLINAS POR CURSO
        // ============================================================

        /// <summary>
        /// Lista disciplinas vinculadas a um curso específico.
        ///
        /// Esse endpoint é utilizado em telas onde o frontend precisa filtrar
        /// a grade acadêmica de acordo com o curso selecionado.
        ///
        /// Exemplo:
        /// ao selecionar um curso no painel administrativo, o sistema pode buscar
        /// apenas as disciplinas pertencentes àquele curso.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet("curso/{cursoId}")]
        public async Task<IActionResult> ListarPorCurso(int cursoId)
        {
            var disciplinas = await _service.ListarPorCurso(cursoId);

            return Ok(disciplinas);
        }

        // ============================================================
        // 4. OBTER DISCIPLINA POR ID
        // ============================================================

        /// <summary>
        /// Obtém os dados de uma disciplina específica pelo ID.
        ///
        /// Esse endpoint pode ser acessado por todos os perfis autenticados,
        /// pois a disciplina pode ser exibida em diferentes contextos:
        /// administração, professor e área acadêmica do aluno.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var disciplina = await _service.ObterPorId(id);

            if (disciplina == null)
                return NotFound();

            return Ok(disciplina);
        }

        // ============================================================
        // 5. ATUALIZAR DISCIPLINA
        // ============================================================

        /// <summary>
        /// Atualiza os dados de uma disciplina existente.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois alterações
        /// em disciplinas impactam a estrutura acadêmica do curso.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarDisciplinaDTO dto)
        {
            var disciplina = await _service.Atualizar(id, dto);

            if (disciplina == null)
                return NotFound();

            return Ok(disciplina);
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente uma disciplina.
        ///
        /// Em vez de excluir fisicamente a disciplina do banco, o sistema pode
        /// marcar o registro como inativo na camada de serviço/repositório.
        ///
        /// Essa abordagem preserva vínculos acadêmicos históricos, como turmas,
        /// aulas, atividades e registros relacionados.
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
        // 7. REATIVAR DISCIPLINA
        // ============================================================

        /// <summary>
        /// Reativa uma disciplina previamente desativada.
        ///
        /// Esse endpoint complementa o soft delete, permitindo que uma disciplina
        /// volte a ser utilizada sem necessidade de recriar seu cadastro.
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