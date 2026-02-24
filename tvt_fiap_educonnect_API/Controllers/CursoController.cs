using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CursoController : ControllerBase
    {
        private readonly ICursoService _service;

        public CursoController(ICursoService service)
        {
            _service = service;
        }

        // Criar curso
        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarCursoDTO dto)
        {
            var curso = await _service.Criar(dto);
            return Ok(curso);
        }

        // Listar cursos (com paginação)
        [Authorize(Roles = "0,1")]
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            return Ok(await _service.Listar(page, pageSize, search));
        }

        // Obter por ID
        [Authorize(Roles = "0,1,2")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var curso = await _service.ObterPorId(id);
            if (curso == null) return NotFound();

            return Ok(curso);
        }

        // Atualizar
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarCursoDTO dto)
        {
            var curso = await _service.Atualizar(id, dto);
            if (curso == null) return NotFound();

            return Ok(curso);
        }

        // 🔥 SOFT DELETE
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var sucesso = await _service.Deletar(id);
            if (!sucesso) return NotFound();

            return NoContent();
        }

        // 🔥 REATIVAR
        [Authorize(Roles = "0,1")]
        [HttpPut("reativar/{id}")]
        public async Task<IActionResult> Reativar(int id)
        {
            var sucesso = await _service.Reativar(id);
            if (!sucesso) return NotFound();

            return NoContent();
        }
    }
}