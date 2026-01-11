using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/aulas")]
    public class AulaController : ControllerBase
    {
        private readonly IAulaService _service;

        public AulaController(IAulaService service)
        {
            _service = service;
        }

        // =========================================================
        // MÉTODO AUXILIAR – OBTER ID DO USUÁRIO PELO TOKEN
        // =========================================================
        private int ObterUsuarioId()
        {
            var usuarioIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(usuarioIdClaim))
                throw new UnauthorizedAccessException("Token não contém o id do usuário.");

            return int.Parse(usuarioIdClaim);
        }

        // =========================================================
        // CRIAR AULA (Professor ou Admin)
        // =========================================================
        [HttpPost]
        [Authorize(Roles = "0,1,2")]
        public async Task<IActionResult> Criar(CriarAulaDTO dto)
        {
            var usuarioId = ObterUsuarioId();

            var aula = await _service.Criar(usuarioId, dto);
            return Ok(aula);
        }

        // =========================================================
        // UPLOAD MATERIAL DE APOIO (PDF)
        // =========================================================
        [HttpPost("{id}/material")]
        [Authorize(Roles = "0,1,2")]
        public async Task<IActionResult> UploadMaterial(int id, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo inválido.");

            var aula = await _service.UploadMaterialApoio(id, arquivo);
            return aula == null ? NotFound() : Ok(aula);
        }

        // =========================================================
        // BAIXAR MATERIAL DE APOIO
        // =========================================================
        [HttpGet("{id}/material")]
        [Authorize]
        public async Task<IActionResult> BaixarMaterial(int id)
        {
            var bytes = await _service.BaixarMaterialApoio(id);
            if (bytes == null) return NotFound();

            return File(bytes, "application/pdf", "material_apoio.pdf");
        }

        // =========================================================
        // OBTER AULA POR ID
        // =========================================================
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Obter(int id)
        {
            var aula = await _service.ObterPorId(id);
            return aula == null ? NotFound() : Ok(aula);
        }

        // =========================================================
        // LISTAR AULAS POR TURMA + DISCIPLINA
        // =========================================================
        [HttpGet("turma-disciplina/{turmaDisciplinaId}")]
        [Authorize]
        public async Task<IActionResult> ListarPorTurmaDisciplina(int turmaDisciplinaId)
        {
            var lista = await _service.ListarPorTurmaDisciplina(turmaDisciplinaId);
            return Ok(lista);
        }

        // =========================================================
        // LISTAR TODAS (ADM)
        // =========================================================
        [HttpGet]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> Listar()
        {
            var lista = await _service.Listar();
            return Ok(lista);
        }

        // =========================================================
        // DELETAR AULA (ADM)
        // =========================================================
        [HttpDelete("{id}")]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> Deletar(int id)
        {
            var ok = await _service.Deletar(id);
            return ok ? NoContent() : NotFound();
        }

        // Minhas Aulas
        [HttpGet("minhas")]
        [Authorize(Roles = "3")] // aluno
        public async Task<IActionResult> ListarMinhasAulas()
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);
            return Ok(await _service.ListarMinhasAulas(usuarioId));
        }

    }
}
