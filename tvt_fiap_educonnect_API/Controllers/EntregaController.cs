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

        [Authorize(Roles = "3")] // aluno entrega
        [HttpPost("{atividadeId}")]
        public async Task<IActionResult> Criar(Guid atividadeId, IFormFile arquivo)
        {
            var usuarioId = Guid.Parse(User.FindFirst("id")!.Value);
            return Ok(await _service.CriarEntrega(usuarioId, atividadeId, arquivo));
        }

        [Authorize(Roles = "2")] // professor corrige
        [HttpPut("{entregaId}/corrigir")]
        public async Task<IActionResult> Corrigir(Guid entregaId, [FromBody] CorrigirDTO dto)
        {
            return Ok(await _service.Corrigir(entregaId, dto.Nota, dto.Feedback));
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("atividade/{atividadeId}")]
        public async Task<IActionResult> ListarPorAtividade(Guid atividadeId)
        {
            return Ok(await _service.ListarPorAtividade(atividadeId));
        }
    }
}