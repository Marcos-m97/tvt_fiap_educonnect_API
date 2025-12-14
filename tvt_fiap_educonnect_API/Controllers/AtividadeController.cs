using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AtividadeController : ControllerBase
    {
        private readonly IAtividadeService _service;

        public AtividadeController(IAtividadeService service)
        {
            _service = service;
        }

        [Authorize(Roles = "0, 2")] // sysADM e Professor
        [HttpPost]
        public async Task<IActionResult> Criar(CriarAtividadeDTO dto)
        {
            return Ok(await _service.Criar(dto));
        }

        [Authorize(Roles = "0,1,2")] // sysADM, ADM e Professor
        [HttpGet("turma-disciplina/{id}")]
        public async Task<IActionResult> Listar(Guid id)
        {
            return Ok(await _service.ListarPorTurmaDisciplina(id));
        }
        [Authorize(Roles = "3")] // aluno
        [HttpGet("minhas")]
        public async Task<IActionResult> ListarMinhasAtividades()
        {
            var usuarioId = Guid.Parse(User.FindFirst("id")!.Value);
            return Ok(await _service.ListarMinhasAtividades(usuarioId));
        }

    }
}

