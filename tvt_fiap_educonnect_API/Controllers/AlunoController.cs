using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _service;

        public AlunoController(IAlunoService service)
        {
            _service = service;
        }

        [Authorize(Roles = "0,1,3")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarAlunoDTO dto)
        {
            var aluno = await _service.Criar(dto);
            return Ok(aluno);
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
            var aluno = await _service.ObterPorUsuario(usuarioId);
            if (aluno == null) return NotFound();

            return Ok(aluno);
        }

        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, CriarAlunoDTO dto)
        {
            var aluno = await _service.Atualizar(id, dto);
            if (aluno == null) return NotFound();

            return Ok(aluno);
        }
    }
}