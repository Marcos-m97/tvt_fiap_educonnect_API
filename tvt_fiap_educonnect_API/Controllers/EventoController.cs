using System.Security.Claims;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados aos eventos do EduConnect.
    ///
    /// No sistema, eventos representam registros de calendário acadêmico ou administrativo,
    /// como provas, atividades, aulas extras, reuniões e avisos gerais.
    ///
    /// Este controller organiza os fluxos de criação, listagem, consulta, atualização,
    /// exclusão e consulta dos eventos disponíveis para o usuário/aluno logado.
    /// As regras de negócio ficam concentradas no EventoService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly IEventoService _service;

        /// <summary>
        /// Recebe o serviço de eventos por injeção de dependência.
        ///
        /// O EventoService concentra as regras de negócio, enquanto o controller
        /// extrai dados da requisição e do token JWT, aplica autorização e retorna
        /// respostas HTTP ao frontend.
        /// </summary>
        public EventoController(IEventoService service)
        {
            _service = service;
        }

        // =========================================================
        // 1. CRIAR EVENTO
        // =========================================================

        /// <summary>
        /// Cria um novo evento.
        ///
        /// Esse endpoint pode ser acessado por SuperAdmin, Admin e Professor.
        /// O ID do usuário criador é obtido pela claim "id" do token JWT
        /// e enviado ao Service para registrar a auditoria de criação.
        ///
        /// O evento pode ser geral, vinculado a uma turma ou vinculado
        /// a uma TurmaDisciplina específica.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarEventoDTO dto)
        {
            var criadorId = int.Parse(User.FindFirst("id")!.Value);

            var evento = await _service.Criar(criadorId, dto);

            return Ok(evento);
        }

        // =========================================================
        // 2. LISTAR TODOS
        // =========================================================

        /// <summary>
        /// Lista todos os eventos cadastrados.
        ///
        /// Esse endpoint exige apenas autenticação e pode ser usado em visões gerais
        /// de calendário, painéis administrativos ou consultas acadêmicas.
        /// </summary>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var eventos = await _service.Listar();

            return Ok(eventos);
        }

        // =========================================================
        // 3. OBTER EVENTO POR ID
        // =========================================================

        /// <summary>
        /// Obtém um evento específico pelo ID.
        ///
        /// Caso o evento não exista, retorna NotFound.
        /// </summary>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var evento = await _service.Obter(id);

            return evento == null ? NotFound() : Ok(evento);
        }

        // =========================================================
        // 4. LISTAR EVENTOS POR TURMA
        // =========================================================

        /// <summary>
        /// Lista os eventos relacionados a uma turma específica.
        ///
        /// O Service considera tanto eventos vinculados diretamente à turma
        /// quanto eventos vinculados a disciplinas pertencentes a essa turma.
        /// </summary>
        [Authorize]
        [HttpGet("turma/{turmaId}")]
        public async Task<IActionResult> ListarPorTurma(int turmaId)
        {
            var eventos = await _service.ListarPorTurma(turmaId);

            return Ok(eventos);
        }

        // =========================================================
        // 5. ATUALIZAR EVENTO
        // =========================================================

        /// <summary>
        /// Atualiza um evento existente.
        ///
        /// Esse endpoint pode ser acessado por SuperAdmin, Admin e Professor.
        /// O ID do usuário e sua role são enviados ao Service, que aplica a regra
        /// de permissão: professores só podem editar eventos criados por eles mesmos.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarEventoDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var evento = await _service.Atualizar(id, usuarioId, role, dto);

            return evento == null ? NotFound() : Ok(evento);
        }

        // =========================================================
        // 6. EVENTOS DO USUÁRIO/ALUNO
        // =========================================================

        /// <summary>
        /// Lista os eventos disponíveis para o usuário logado.
        ///
        /// No fluxo do aluno, o Service identifica o aluno pelo usuário autenticado,
        /// verifica sua matrícula ativa e retorna os eventos da turma correspondente.
        ///
        /// Esse endpoint é usado principalmente para montar a agenda/calendário
        /// individual do aluno.
        /// </summary>
        [Authorize]
        [HttpGet("meus")]
        public async Task<IActionResult> ListarMeusEventos()
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);

            var eventos = await _service.ListarMeusEventos(usuarioId);

            return Ok(eventos);
        }

        // =========================================================
        // 7. DELETAR EVENTO
        // =========================================================

        /// <summary>
        /// Remove um evento.
        ///
        /// Esse endpoint pode ser acessado por SuperAdmin, Admin e Professor.
        /// Assim como na atualização, o Service aplica a regra de permissão:
        /// professores só podem deletar eventos criados por eles mesmos.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);
            var role = User.FindFirst(ClaimTypes.Role)!.Value;

            var ok = await _service.Deletar(id, usuarioId, role);

            return ok ? NoContent() : NotFound();
        }
    }
}