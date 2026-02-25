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
            var alunoId = int.Parse(User.FindFirst("id")!.Value);

            var matricula = await _service.Criar(alunoId, dto);

            return Ok(matricula);
        }

        // =========================================================
        // 2. Obter matrícula por ID
        // =========================================================
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var m = await _service.ObterPorId(id);
            if (m == null) return NotFound();

            return Ok(m);
        }

        // =========================================================
        // 3. Listar matrículas paginado com filtros (Admin)
        // =========================================================
        [Authorize(Roles = "0,1")]
        [HttpGet]
        public async Task<IActionResult> Listar(
            int page = 1,
            int pageSize = 10,
            string? search = null,
            MatriculaStatus? status = null)
        {
            var result = await _service.ListarPaginado(
                page,
                pageSize,
                search,
                status
            );

            return Ok(result);
        }

        // =========================================================
        // 4. Listar matrículas por aluno
        // =========================================================
        [Authorize]
        [HttpGet("aluno/{alunoId}")]
        public async Task<IActionResult> ListarPorAluno(int alunoId)
        {
            return Ok(await _service.ListarPorAluno(alunoId));
        }

        // =========================================================
        // 5. Listar matrículas por turma
        // =========================================================
        [Authorize(Roles = "0,1,2")]
        [HttpGet("turma/{turmaId}")]
        public async Task<IActionResult> ListarPorTurma(int turmaId)
        {
            return Ok(await _service.ListarPorTurma(turmaId));
        }

        // =========================================================
        // 6. Atualizar status (Administradores)
        // =========================================================
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}/status/{novoStatus}")]
        public async Task<IActionResult> AtualizarStatus(int id, MatriculaStatus novoStatus)
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
        public async Task<IActionResult> Deletar(int id)
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
        public async Task<IActionResult> UploadComprovante(int id, IFormFile arquivo)
        {
            var result = await _service.UploadComprovantePagamento(id, arquivo);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "3")]
        [HttpPut("{id}/upload-documentos-pessoais")]
        public async Task<IActionResult> UploadDocumentosPessoais(int id, IFormFile arquivo)
        {
            var result = await _service.UploadDocumentosPessoais(id, arquivo);
            if (result == null) return NotFound();
            return Ok(result);
        }

        [Authorize(Roles = "3")]
        [HttpPut("{id}/upload-documentos-escolaridade")]
        public async Task<IActionResult> UploadDocumentosEscolaridade(int id, IFormFile arquivo)
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
        public async Task<IActionResult> DownloadComprovante(int id)
        {
            var bytes = await _service.BaixarComprovante(id);
            if (bytes == null) return NotFound();

            return File(bytes, "application/pdf", "comprovante.pdf");
        }

        [Authorize(Roles = "0,1")]
        [HttpGet("{id}/download/documentos-pessoais")]
        public async Task<IActionResult> DownloadDocumentosPessoais(int id)
        {
            var bytes = await _service.BaixarDocumentosPessoais(id);
            if (bytes == null) return NotFound();

            return File(bytes, "application/pdf", "documentos_pessoais.pdf");
        }

        [Authorize(Roles = "0,1")]
        [HttpGet("{id}/download/documentos-escolaridade")]
        public async Task<IActionResult> DownloadDocumentosEscolaridade(int id)
        {
            var bytes = await _service.BaixarDocumentosEscolaridade(id);
            if (bytes == null) return NotFound();

            return File(bytes, "application/pdf", "documentos_escolaridade.pdf");
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet("turma/{turmaId}/alunos")]
        public async Task<IActionResult> ListarAlunosDaTurma(
            int turmaId,
            int page = 1,
            int pageSize = 5,
            string? search = null)
        {
            var result = await _service.ListarAlunosPorTurma(
                turmaId, page, pageSize, search);

            return Ok(result);
        }
        [Authorize(Roles = "0,1")]
        [HttpGet("aluno/{alunoId}/ativa")]
        public async Task<IActionResult> ObterMatriculaAtiva(int alunoId)
        {
            var matricula = await _service.ObterAtivaPorAlunoId(alunoId);
            if (matricula == null)
                return NotFound();

            return Ok(matricula);
        }

    }
}