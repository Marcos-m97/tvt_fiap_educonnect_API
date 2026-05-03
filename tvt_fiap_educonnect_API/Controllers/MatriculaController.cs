using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados ao processo de matrícula.
    ///
    /// No EduConnect, a matrícula representa o vínculo entre um aluno e uma turma,
    /// além de controlar as etapas necessárias para que o aluno tenha acesso completo
    /// ao ambiente acadêmico.
    ///
    /// Este controller expõe operações para criação de matrícula, consulta,
    /// listagem administrativa, upload e download de documentos, atualização de status
    /// e listagem de alunos matriculados por turma.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MatriculaController : ControllerBase
    {
        private readonly IMatriculaService _service;

        /// <summary>
        /// Recebe o serviço de matrícula por injeção de dependência.
        ///
        /// O MatriculaService concentra as regras de negócio do fluxo de matrícula,
        /// enquanto o controller fica responsável por receber requisições HTTP
        /// e retornar respostas ao frontend.
        /// </summary>
        public MatriculaController(IMatriculaService service)
        {
            _service = service;
        }

        // =========================================================
        // 1. CRIAR MATRÍCULA
        // =========================================================

        /// <summary>
        /// Cria uma nova solicitação de matrícula para o aluno logado.
        ///
        /// Esse endpoint é usado quando o aluno escolhe uma turma e inicia
        /// o processo de matrícula.
        ///
        /// O ID do usuário autenticado é recuperado do token JWT pela claim "id".
        /// A partir dele, o Service localiza o perfil de aluno correspondente
        /// e cria a matrícula na turma informada.
        /// </summary>
        [Authorize(Roles = "0,3")]
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarMatriculaDTO dto)
        {
            var alunoId = int.Parse(User.FindFirst("id")!.Value);

            var matricula = await _service.Criar(alunoId, dto);

            return Ok(matricula);
        }

        // =========================================================
        // 2. OBTER MATRÍCULA POR ID
        // =========================================================

        /// <summary>
        /// Obtém uma matrícula específica pelo ID.
        ///
        /// Esse endpoint pode ser acessado por todos os perfis autenticados,
        /// pois a matrícula pode ser consultada em diferentes contextos:
        /// administração, professor e área do aluno.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var m = await _service.ObterPorId(id);

            if (m == null)
                return NotFound();

            return Ok(m);
        }

        // =========================================================
        // 3. LISTAR MATRÍCULAS PAGINADO COM FILTROS
        // =========================================================

        /// <summary>
        /// Lista matrículas de forma paginada, com filtros opcionais.
        ///
        /// Esse endpoint é voltado ao painel administrativo, permitindo consultar
        /// solicitações de matrícula sem carregar todos os registros de uma vez.
        ///
        /// Os filtros permitem buscar pelo nome do aluno e também filtrar pelo
        /// status atual da matrícula.
        /// </summary>
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
        // 4. LISTAR MATRÍCULAS POR ALUNO
        // =========================================================

        /// <summary>
        /// Lista todas as matrículas vinculadas a um aluno.
        ///
        /// Esse endpoint é usado principalmente na visão do aluno,
        /// para que ele acompanhe suas solicitações de matrícula e o status
        /// de cada uma.
        /// </summary>
        [Authorize]
        [HttpGet("aluno/{alunoId}")]
        public async Task<IActionResult> ListarPorAluno(int alunoId)
        {
            var result = await _service.ListarPorAluno(alunoId);

            return Ok(result);
        }

        // =========================================================
        // 5. LISTAR MATRÍCULAS POR TURMA
        // =========================================================

        /// <summary>
        /// Lista todas as matrículas de uma turma específica.
        ///
        /// Esse endpoint pode ser usado por administradores e professores para
        /// acompanhar os alunos vinculados a determinada turma, independentemente
        /// do status da matrícula.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet("turma/{turmaId}")]
        public async Task<IActionResult> ListarPorTurma(int turmaId)
        {
            var result = await _service.ListarPorTurma(turmaId);

            return Ok(result);
        }

        // =========================================================
        // 6. ATUALIZAR STATUS DA MATRÍCULA
        // =========================================================

        /// <summary>
        /// Atualiza o status de uma matrícula.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois representa uma ação
        /// administrativa dentro do fluxo de matrícula.
        ///
        /// Exemplo de uso:
        /// avançar uma matrícula para Pagamento, Documentos ou Efetivada.
        ///
        /// Quando o status passa para Efetivada, o Service executa a regra de negócio
        /// de enviar e-mail de confirmação ao aluno.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}/status/{novoStatus}")]
        public async Task<IActionResult> AtualizarStatus(int id, MatriculaStatus novoStatus)
        {
            var m = await _service.AtualizarStatus(id, novoStatus);

            if (m == null)
                return NotFound();

            return Ok(m);
        }

        // =========================================================
        // 7. DELETAR MATRÍCULA
        // =========================================================

        /// <summary>
        /// Remove uma matrícula.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin.
        /// No fluxo atual, a matrícula é removida fisicamente pelo Repository.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Deletar(int id)
        {
            var ok = await _service.Deletar(id);

            if (!ok)
                return NotFound();

            return NoContent();
        }

        // =========================================================
        // 8. UPLOAD DO COMPROVANTE DE PAGAMENTO
        // =========================================================

        /// <summary>
        /// Realiza o upload do comprovante de pagamento da matrícula.
        ///
        /// Esse endpoint é destinado ao aluno. Após o envio, o Service salva
        /// o arquivo e atualiza a matrícula para a etapa de Pagamento.
        /// </summary>
        [Authorize(Roles = "3")]
        [HttpPut("{id}/upload-comprovante")]
        public async Task<IActionResult> UploadComprovante(int id, IFormFile arquivo)
        {
            var result = await _service.UploadComprovantePagamento(id, arquivo);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // =========================================================
        // 9. UPLOAD DOS DOCUMENTOS PESSOAIS
        // =========================================================

        /// <summary>
        /// Realiza o upload dos documentos pessoais da matrícula.
        ///
        /// Esse endpoint é usado pelo aluno durante a etapa documental.
        /// O arquivo é salvo pelo Service e vinculado à matrícula.
        /// </summary>
        [Authorize(Roles = "3")]
        [HttpPut("{id}/upload-documentos-pessoais")]
        public async Task<IActionResult> UploadDocumentosPessoais(int id, IFormFile arquivo)
        {
            var result = await _service.UploadDocumentosPessoais(id, arquivo);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // =========================================================
        // 10. UPLOAD DOS DOCUMENTOS DE ESCOLARIDADE
        // =========================================================

        /// <summary>
        /// Realiza o upload dos documentos de escolaridade da matrícula.
        ///
        /// Esse arquivo complementa a etapa documental exigida para validação
        /// da matrícula do aluno.
        /// </summary>
        [Authorize(Roles = "3")]
        [HttpPut("{id}/upload-documentos-escolaridade")]
        public async Task<IActionResult> UploadDocumentosEscolaridade(int id, IFormFile arquivo)
        {
            var result = await _service.UploadDocumentosEscolaridade(id, arquivo);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        // =========================================================
        // 11. DOWNLOAD DO COMPROVANTE DE PAGAMENTO
        // =========================================================

        /// <summary>
        /// Baixa o comprovante de pagamento vinculado à matrícula.
        ///
        /// O endpoint retorna o arquivo como PDF para que administradores
        /// ou o próprio aluno possam consultar o documento enviado.
        /// </summary>
        [Authorize(Roles = "0,1,3")]
        [HttpGet("{id}/download/comprovante")]
        public async Task<IActionResult> DownloadComprovante(int id)
        {
            var bytes = await _service.BaixarComprovante(id);

            if (bytes == null)
                return NotFound();

            return File(bytes, "application/pdf", "comprovante.pdf");
        }

        // =========================================================
        // 12. DOWNLOAD DOS DOCUMENTOS PESSOAIS
        // =========================================================

        /// <summary>
        /// Baixa os documentos pessoais vinculados à matrícula.
        ///
        /// O endpoint retorna o arquivo como PDF.
        /// </summary>
        [Authorize(Roles = "0,1,3")]
        [HttpGet("{id}/download/documentos-pessoais")]
        public async Task<IActionResult> DownloadDocumentosPessoais(int id)
        {
            var bytes = await _service.BaixarDocumentosPessoais(id);

            if (bytes == null)
                return NotFound();

            return File(bytes, "application/pdf", "documentos_pessoais.pdf");
        }

        // =========================================================
        // 13. DOWNLOAD DOS DOCUMENTOS DE ESCOLARIDADE
        // =========================================================

        /// <summary>
        /// Baixa os documentos de escolaridade vinculados à matrícula.
        ///
        /// O endpoint retorna o arquivo como PDF.
        /// </summary>
        [Authorize(Roles = "0,1,3")]
        [HttpGet("{id}/download/documentos-escolaridade")]
        public async Task<IActionResult> DownloadDocumentosEscolaridade(int id)
        {
            var bytes = await _service.BaixarDocumentosEscolaridade(id);

            if (bytes == null)
                return NotFound();

            return File(bytes, "application/pdf", "documentos_escolaridade.pdf");
        }

        // =========================================================
        // 14. LISTAR ALUNOS DA TURMA
        // =========================================================

        /// <summary>
        /// Lista alunos efetivados em uma turma, com paginação e busca opcional.
        ///
        /// Esse endpoint é útil para professores e administradores visualizarem
        /// os alunos ativos de uma turma específica.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet("turma/{turmaId}/alunos")]
        public async Task<IActionResult> ListarAlunosDaTurma(
            int turmaId,
            int page = 1,
            int pageSize = 5,
            string? search = null)
        {
            var result = await _service.ListarAlunosPorTurma(
                turmaId,
                page,
                pageSize,
                search
            );

            return Ok(result);
        }

        // =========================================================
        // 15. OBTER MATRÍCULA ATIVA DO ALUNO
        // =========================================================

        /// <summary>
        /// Obtém a matrícula ativa/efetivada de um aluno.
        ///
        /// Esse endpoint é usado para verificar se o aluno já possui uma matrícula
        /// efetivada e, com isso, liberar o acesso às funcionalidades acadêmicas
        /// do portal.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
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