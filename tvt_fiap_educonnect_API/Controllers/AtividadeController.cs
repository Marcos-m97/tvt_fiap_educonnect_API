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

        [Authorize(Roles = "2")] // professor
        [HttpPost]
        public async Task<IActionResult> Criar(CriarAtividadeDTO dto)
        {
            return Ok(await _service.Criar(dto));
        }

        [HttpGet("turma-disciplina/{id}")]
        public async Task<IActionResult> Listar(Guid id)
        {
            return Ok(await _service.ListarPorTurmaDisciplina(id));
        }
    }
}

