using System.Security.Claims;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventoController : ControllerBase
    {
        private readonly IEventoService _service;

        public EventoController(IEventoService service)
        {
            _service = service;
        }

        // =========================================================
        // CRIAR EVENTO
        // =========================================================
        [Authorize(Roles = "0,1,2")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarEventoDTO dto)
        {
            var criadorId = int.Parse(User.FindFirst("id")!.Value);

            var evento = await _service.Criar(criadorId, dto);
            return Ok(evento);
        }

        // =========================================================
        // LISTAR TODOS
        // =========================================================
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        // =========================================================
        // OBTER POR ID
        // =========================================================
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var evento = await _service.Obter(id);
            return evento == null ? NotFound() : Ok(evento);
        }

        // =========================================================
        // LISTAR POR TURMA
        // =========================================================
        [Authorize]
        [HttpGet("turma/{turmaId}")]
        public async Task<IActionResult> ListarPorTurma(int turmaId)
        {
            return Ok(await _service.ListarPorTurma(turmaId));
        }

        // =========================================================
        // ATUALIZAR
        // =========================================================
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
        // EVENTOS DO ALUNO
        // =========================================================
        [Authorize]
        [HttpGet("meus")]
        public async Task<IActionResult> ListarMeusEventos()
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);

            return Ok(await _service.ListarMeusEventos(usuarioId));
        }

        // =========================================================
        // DELETAR
        // =========================================================
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