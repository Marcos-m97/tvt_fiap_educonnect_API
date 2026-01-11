using System.Security.Claims;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntregaController : ControllerBase
    {
        private readonly IEntregaService _service;

        public EntregaController(IEntregaService service)
        {
            _service = service;
        }

        [Authorize(Roles = "3")] // Aluno entrega
        [HttpPost("{atividadeId}")]
        public async Task<IActionResult> Criar(int atividadeId, IFormFile arquivo)
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);
            return Ok(await _service.CriarEntrega(usuarioId, atividadeId, arquivo));
        }

        [Authorize(Roles = "2")] // Professor corrige
        [HttpPut("{entregaId}/corrigir")]
        public async Task<IActionResult> Corrigir(int entregaId, [FromBody] CorrigirDTO dto)
        {
            return Ok(await _service.Corrigir(entregaId, dto.Nota, dto.Feedback));
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet("atividade/{atividadeId}")]
        public async Task<IActionResult> ListarPorAtividade(int atividadeId)
        {
            return Ok(await _service.ListarPorAtividade(atividadeId));
        }

        [Authorize(Roles = "3")] // Aluno busca entregas
        [HttpGet("minhas")]
        public async Task<IActionResult> ListarMinhasEntregas(
        [FromQuery] int? disciplinaId,
        [FromQuery] int? atividadeId)
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);
            return Ok(await _service.ListarMinhasEntregas(usuarioId, disciplinaId, atividadeId));
        }
    }
}