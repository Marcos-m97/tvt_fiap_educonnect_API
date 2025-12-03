using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TurmaDisciplinaController : ControllerBase
    {
        private readonly ITurmaDisciplinaService _service;

        public TurmaDisciplinaController(ITurmaDisciplinaService service)
        {
            _service = service;
        }

        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarTurmaDisciplinaDTO dto)
        {
            var result = await _service.Criar(dto);
            return Ok(result);
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var result = await _service.ObterPorId(id);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("turma/{turmaId}")]
        public async Task<IActionResult> ListarPorTurma(Guid turmaId)
        {
            return Ok(await _service.ListarPorTurma(turmaId));
        }

        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, CriarTurmaDisciplinaDTO dto)
        {
            var result = await _service.Atualizar(id, dto);
            if (result == null) return NotFound();

            return Ok(result);
        }

        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            var ok = await _service.Deletar(id);
            if (!ok) return NotFound();

            return NoContent();
        }
    }
}