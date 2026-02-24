using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurmaController : ControllerBase
    {
        private readonly ITurmaService _service;

        public TurmaController(ITurmaService service)
        {
            _service = service;
        }

        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarTurmaDTO dto)
        {
            var turma = await _service.Criar(dto);
            return Ok(turma);
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var turma = await _service.ObterPorId(id);
            if (turma == null) return NotFound();

            return Ok(turma);
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("curso/{cursoId}")]
        public async Task<IActionResult> ListarPorCurso(int cursoId)
        {
            return Ok(await _service.ListarPorCurso(cursoId));
        }

        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarTurmaDTO dto)
        {
            var turma = await _service.Atualizar(id, dto);
            if (turma == null) return NotFound();

            return Ok(turma);
        }

        // 🔥 SOFT DELETE
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var ok = await _service.Deletar(id);
            if (!ok) return NotFound();

            return NoContent();
        }

        // 🔥 REATIVAR
        [Authorize(Roles = "0,1")]
        [HttpPut("reativar/{id}")]
        public async Task<IActionResult> Reativar(int id)
        {
            var ok = await _service.Reativar(id);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}