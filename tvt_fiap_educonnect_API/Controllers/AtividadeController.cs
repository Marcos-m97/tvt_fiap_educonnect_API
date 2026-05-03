using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados às atividades acadêmicas.
    ///
    /// No EduConnect, a atividade representa uma tarefa, prova, trabalho ou exercício
    /// criado dentro de uma TurmaDisciplina.
    ///
    /// Este controller expõe operações para criação, listagem por turma/disciplina,
    /// listagem das atividades do aluno, consulta por ID e atualização.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AtividadeController : ControllerBase
    {
        private readonly IAtividadeService _service;

        /// <summary>
        /// Recebe o serviço de atividades por injeção de dependência.
        ///
        /// O AtividadeService concentra as regras de negócio, enquanto o controller
        /// recebe as requisições HTTP, extrai dados do token quando necessário
        /// e retorna as respostas ao frontend.
        /// </summary>
        public AtividadeController(IAtividadeService service)
        {
            _service = service;
        }

        // ============================================================
        // 1. CRIAR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Cria uma nova atividade acadêmica.
        ///
        /// Esse endpoint é destinado a SuperAdmin e Professor.
        /// A atividade é vinculada a uma TurmaDisciplina, ou seja,
        /// a uma disciplina ofertada dentro de uma turma específica.
        /// </summary>
        [Authorize(Roles = "0, 2")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarAtividadeDTO dto)
        {
            var atividade = await _service.Criar(dto);

            return Ok(atividade);
        }

        // ============================================================
        // 2. LISTAR ATIVIDADES POR TURMA/DISCIPLINA
        // ============================================================

        /// <summary>
        /// Lista as atividades vinculadas a uma TurmaDisciplina específica.
        ///
        /// Esse endpoint é usado principalmente na visão do professor ou da gestão
        /// acadêmica, permitindo consultar as atividades de uma disciplina
        /// dentro de uma turma.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet("turma-disciplina/{id}")]
        public async Task<IActionResult> Listar(int id)
        {
            var atividades = await _service.ListarPorTurmaDisciplina(id);

            return Ok(atividades);
        }

        // ============================================================
        // 3. LISTAR MINHAS ATIVIDADES
        // ============================================================

        /// <summary>
        /// Lista as atividades disponíveis para o aluno logado.
        ///
        /// Esse endpoint é exclusivo para alunos. O ID do usuário é recuperado
        /// da claim "id" do token JWT, e o Service usa esse valor para localizar
        /// o aluno, sua matrícula ativa e as atividades da turma correspondente.
        ///
        /// O retorno também informa se cada atividade já foi entregue e a nota,
        /// quando houver entrega avaliada.
        /// </summary>
        [Authorize(Roles = "3")]
        [HttpGet("minhas")]
        public async Task<IActionResult> ListarMinhasAtividades()
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);

            var atividades = await _service.ListarMinhasAtividades(usuarioId);

            return Ok(atividades);
        }

        // ============================================================
        // 4. OBTER ATIVIDADE POR ID
        // ============================================================

        /// <summary>
        /// Obtém uma atividade específica pelo ID.
        ///
        /// Esse endpoint pode ser acessado por todos os perfis autenticados,
        /// pois a atividade pode ser consultada tanto na visão administrativa
        /// quanto na visão do professor e do aluno.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var atividade = await _service.ObterPorId(id);

            return Ok(atividade);
        }

        // ============================================================
        // 5. ATUALIZAR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Atualiza uma atividade acadêmica existente.
        ///
        /// Esse endpoint é destinado a SuperAdmin, Admin e Professor.
        /// Permite alterar dados como título, descrição, data de entrega,
        /// tipo e URL de material.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, AtualizarAtividadeDTO dto)
        {
            var atividade = await _service.Atualizar(id, dto);

            return Ok(atividade);
        }
    }
}