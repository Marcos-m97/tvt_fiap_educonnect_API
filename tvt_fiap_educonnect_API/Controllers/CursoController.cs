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

        // Listar todos
        [Authorize(Roles = "0,1,2")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        // Obter por ID
        [Authorize(Roles = "0,1,2")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var curso = await _service.ObterPorId(id);
            if (curso == null) return NotFound();

            return Ok(curso);
        }

        // Atualizar
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, CriarCursoDTO dto)
        {
            var curso = await _service.Atualizar(id, dto);
            if (curso == null) return NotFound();

            return Ok(curso);
        }

        // Deletar
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            var sucesso = await _service.Deletar(id);
            if (!sucesso) return NotFound();

            return NoContent();
        }
    }
}