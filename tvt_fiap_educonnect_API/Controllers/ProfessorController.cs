using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _service;

        public ProfessorController(IProfessorService service)
        {
            _service = service;
        }

        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarProfessorDTO dto)
        {
            var professor = await _service.Criar(dto);
            return Ok(professor);
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Obter(Guid usuarioId)
        {
            var prof = await _service.ObterPorUsuario(usuarioId);
            if (prof == null) return NotFound();

            return Ok(prof);
        }

        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, CriarProfessorDTO dto)
        {
            var prof = await _service.Atualizar(id, dto);
            if (prof == null) return NotFound();

            return Ok(prof);
        }
    }
}
