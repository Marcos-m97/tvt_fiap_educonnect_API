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

        // Criar evento (professores, admins, superadmin)
        [Authorize(Roles = "0,1,2")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarEventoDTO dto)
        {
            var criadorId = Guid.Parse(User.FindFirst("id")!.Value);
            var evento = await _service.Criar(criadorId, dto);
            return Ok(evento);
        }

        // Listar todos
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        // Obter por ID
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var evento = await _service.Obter(id);
            return evento == null ? NotFound() : Ok(evento);
        }

        // Listar eventos por turma
        [Authorize]
        [HttpGet("turma/{turmaId}")]
        public async Task<IActionResult> ListarPorTurma(Guid turmaId)
        {
            return Ok(await _service.ListarPorTurma(turmaId));
        }

        // Atualizar
        [Authorize(Roles = "0,1,2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, CriarEventoDTO dto)
        {
            var evento = await _service.Atualizar(id, dto);
            return evento == null ? NotFound() : Ok(evento);
        }

        // =========================================================
        // EVENTOS DO ALUNO (via token)
        // =========================================================
        [Authorize]
        [HttpGet("meus")]
        public async Task<IActionResult> ListarMeusEventos()
        {
            var usuarioId = Guid.Parse(User.FindFirst("id")!.Value);
            return Ok(await _service.ListarMeusEventos(usuarioId));
        }

        // Deletar
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            var ok = await _service.Deletar(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
