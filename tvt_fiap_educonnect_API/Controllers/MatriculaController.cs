using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MatriculaController : ControllerBase
    {
        private readonly IMatriculaService _service;

        public MatriculaController(IMatriculaService service)
        {
            _service = service;
        }

        // =========================================================
        // 1. Criar matrícula (Aluno solicita matrícula)
        // =========================================================
        [Authorize(Roles = "0,3")] // somente aluno
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarMatriculaDTO dto)
        {
            var alunoId = Guid.Parse(User.FindFirst("id")!.Value);

            var matricula = await _service.Criar(alunoId, dto);

            return Ok(matricula);
        }

        // =========================================================
        // 2. Obter matrícula por ID
        // =========================================================
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(Guid id)
        {
            var m = await _service.ObterPorId(id);
            if (m == null) return NotFound();

            return Ok(m);
        }

        // =========================================================
        // 3. Listar todas as matrículas (somente Admin ou SuperAdmin)
        // =========================================================
        [Authorize(Roles = "0,1")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        // =========================================================
        // 4. Listar matrículas por aluno
        // =========================================================
        [Authorize(Roles = "0,1,3")]
        [HttpGet("aluno/{alunoId}")]
        public async Task<IActionResult> ListarPorAluno(Guid alunoId)
        {
            return Ok(await _service.ListarPorAluno(alunoId));
        }

        // =========================================================
        // 5. Listar matrículas por turma
        // =========================================================
        [Authorize(Roles = "0,1,2")]
        [HttpGet("turma/{turmaId}")]
        public async Task<IActionResult> ListarPorTurma(Guid turmaId)
        {
            return Ok(await _service.ListarPorTurma(turmaId));
        }

        // =========================================================
        // 6. Atualizar status (Administradores)
        // =========================================================
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}/status/{novoStatus}")]
        public async Task<IActionResult> AtualizarStatus(Guid id, MatriculaStatus novoStatus)
        {
            var m = await _service.AtualizarStatus(id, novoStatus);
            if (m == null) return NotFound();

            return Ok(m);
        }

        // =========================================================
        // 7. Remover matrícula (apagar)
        // =========================================================
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(Guid id)
        {
            var ok = await _service.Deletar(id);
            if (!ok) return NotFound();

            return NoContent();
        }

        // =============================
        // UPLOADS
        // =============================
        [Authorize(Roles = "3")]
        [HttpPut("{id}/upload-comprovante")]
        public async Task<IActionResult> UploadComprovante(Guid id, IFormFile arquivo)
        {
            var result = await _service.UploadComprovantePagamento(id, arquivo);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "3")]
        [HttpPut("{id}/upload-documentos-pessoais")]
        public async Task<IActionResult> UploadDocumentosPessoais(Guid id, IFormFile arquivo)
        {
            var result = await _service.UploadDocumentosPessoais(id, arquivo);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "3")]
        [HttpPut("{id}/upload-documentos-escolaridade")]
        public async Task<IActionResult> UploadDocumentosEscolaridade(Guid id, IFormFile arquivo)
        {
            var result = await _service.UploadDocumentosEscolaridade(id, arquivo);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // =============================
        // DOWNLOADS
        // =============================
        [Authorize(Roles = "0,1")]
        [HttpGet("{id}/download/comprovante")]
        public async Task<IActionResult> DownloadComprovante(Guid id)
        {
            var bytes = await _service.BaixarComprovante(id);
            if (bytes == null) return NotFound();

            return File(bytes, "application/pdf", "comprovante.pdf");
        }

        [Authorize(Roles = "0,1")]
        [HttpGet("{id}/download/documentos-pessoais")]
        public async Task<IActionResult> DownloadDocumentosPessoais(Guid id)
        {
            var bytes = await _service.BaixarDocumentosPessoais(id);
            if (bytes == null) return NotFound();

            return File(bytes, "application/pdf", "documentos_pessoais.pdf");
        }

        [Authorize(Roles = "0,1")]
        [HttpGet("{id}/download/documentos-escolaridade")]
        public async Task<IActionResult> DownloadDocumentosEscolaridade(Guid id)
        {
            var bytes = await _service.BaixarDocumentosEscolaridade(id);
            if (bytes == null) return NotFound();

            return File(bytes, "application/pdf", "documentos_escolaridade.pdf");
        }

        // =========================================================
        // 8. Listar alunos da turma (Professor / Admin)
        // =========================================================
        [Authorize(Roles = "0,1,2")]
        [HttpGet("turma/{turmaId}/alunos")]
        public async Task<IActionResult> ListarAlunosDaTurma(Guid turmaId)
        {
            return Ok(await _service.ListarAlunosPorTurma(turmaId));
        }

    }
}