using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/boletins")]
    public class BoletimController : ControllerBase
    {
        private readonly IBoletimService _service;

        public BoletimController(IBoletimService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Gerar(CreateBoletimDTO dto)
        {
            return Ok(await _service.Gerar(dto));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var result = await _service.Obter(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet("aluno/{alunoId}")]
        public async Task<IActionResult> ListarPorAluno(int alunoId)
        {
            return Ok(await _service.ListarPorAluno(alunoId));
        }

        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> Pdf(int id)
        {
            var pdf = await _service.GerarPdf(id);
            return File(pdf, "application/pdf", $"boletim-{id}.pdf");
        }
    }
}
