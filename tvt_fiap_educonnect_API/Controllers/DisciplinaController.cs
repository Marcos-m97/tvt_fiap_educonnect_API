using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DisciplinaController : ControllerBase
    {
        private readonly IDisciplinaService _service;

        public DisciplinaController(IDisciplinaService service)
        {
            _service = service;
        }

        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarDisciplinaDTO dto)
        {
            var result = await _service.Criar(dto);
            return Ok(result);
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet("curso/{cursoId}")]
        public async Task<IActionResult> ListarPorCurso(int cursoId)
        {
            return Ok(await _service.ListarPorCurso(cursoId));
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var disciplina = await _service.ObterPorId(id);
            if (disciplina == null) return NotFound();

            return Ok(disciplina);
        }

        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarDisciplinaDTO dto)
        {
            var disciplina = await _service.Atualizar(id, dto);
            if (disciplina == null) return NotFound();

            return Ok(disciplina);
        }

        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var ok = await _service.Deletar(id);
            if (!ok) return NotFound();

            return NoContent();
        }
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