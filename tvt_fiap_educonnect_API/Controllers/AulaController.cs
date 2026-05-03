using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados às aulas.
    ///
    /// No EduConnect, a aula representa um conteúdo acadêmico vinculado a uma
    /// TurmaDisciplina, ou seja, a uma disciplina ofertada em uma turma específica.
    ///
    /// Este controller expõe operações para criação, atualização, listagem,
    /// upload de material de apoio, upload de vídeo, download de material
    /// e consulta das aulas disponíveis para o aluno.
    /// </summary>
    [ApiController]
    [Route("api/aulas")]
    public class AulaController : ControllerBase
    {
        private readonly IAulaService _service;

        /// <summary>
        /// Recebe o serviço de aulas por injeção de dependência.
        ///
        /// O AulaService concentra as regras de negócio, enquanto o controller
        /// apenas recebe requisições HTTP, extrai dados do token quando necessário
        /// e retorna respostas ao frontend.
        /// </summary>
        public AulaController(IAulaService service)
        {
            _service = service;
        }

        // =========================================================
        // 1. MÉTODO AUXILIAR – OBTER ID DO USUÁRIO PELO TOKEN
        // =========================================================

        /// <summary>
        /// Obtém o ID do usuário autenticado a partir do token JWT.
        ///
        /// A aplicação grava o identificador do usuário na claim "id".
        /// Esse valor é utilizado para registrar quem criou ou atualizou aulas
        /// e também para consultar as aulas disponíveis ao aluno logado.
        /// </summary>
        private int ObterUsuarioId()
        {
            var usuarioIdClaim = User.FindFirst("id")?.Value;

            if (string.IsNullOrEmpty(usuarioIdClaim))
                throw new UnauthorizedAccessException("Token não contém o id do usuário.");

            return int.Parse(usuarioIdClaim);
        }

        // =========================================================
        // 2. CRIAR AULA
        // =========================================================

        /// <summary>
        /// Cria uma nova aula.
        ///
        /// Esse endpoint é permitido para SuperAdmin, Admin e Professor.
        /// O ID do usuário logado é extraído do token e enviado ao Service,
        /// que valida se o usuário possui permissão e cria a aula dentro
        /// de uma TurmaDisciplina válida.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "0,1,2")]
        public async Task<IActionResult> Criar(CriarAulaDTO dto)
        {
            var usuarioId = ObterUsuarioId();

            var aula = await _service.Criar(usuarioId, dto);

            return Ok(aula);
        }

        // =========================================================
        // 3. UPLOAD MATERIAL DE APOIO
        // =========================================================

        /// <summary>
        /// Realiza o upload do material de apoio da aula.
        ///
        /// O arquivo é recebido via multipart/form-data e enviado ao Service,
        /// que salva o documento e atualiza o caminho no registro da aula.
        ///
        /// Esse endpoint é permitido para SuperAdmin, Admin e Professor.
        /// </summary>
        [HttpPost("{id}/material")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "0,1,2")]
        public async Task<IActionResult> UploadMaterial(int id, [FromForm] UploadArquivoDTO dto)
        {
            var arquivo = dto.Arquivo;

            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo inválido.");

            var aula = await _service.UploadMaterialApoio(id, arquivo);

            return aula == null ? NotFound() : Ok(aula);
        }

        // =========================================================
        // 4. UPLOAD VIDEO AULA
        // =========================================================

        /// <summary>
        /// Realiza o upload do vídeo principal da aula.
        ///
        /// O arquivo é recebido via multipart/form-data. O controller faz uma
        /// validação inicial para aceitar apenas arquivos MP4, e o Service
        /// realiza o salvamento e atualização da aula.
        ///
        /// Esse endpoint é permitido para SuperAdmin, Admin e Professor.
        /// </summary>
        [HttpPost("{id}/video")]
        [Consumes("multipart/form-data")]
        [Authorize(Roles = "0,1,2")]
        public async Task<IActionResult> UploadVideo(int id, [FromForm] UploadArquivoDTO dto)
        {
            var arquivo = dto.Arquivo;

            if (arquivo == null || arquivo.Length == 0)
                return BadRequest("Arquivo inválido.");

            if (!arquivo.FileName.EndsWith(".mp4"))
                return BadRequest("Apenas arquivos MP4 são permitidos.");

            var aula = await _service.UploadVideoAula(id, arquivo);

            return aula == null ? NotFound() : Ok(aula);
        }

        // =========================================================
        // 5. BAIXAR MATERIAL DE APOIO
        // =========================================================

        /// <summary>
        /// Baixa o material de apoio vinculado à aula.
        ///
        /// O endpoint retorna o arquivo como PDF para que professores, alunos
        /// ou administradores possam acessar o material disponibilizado.
        /// </summary>
        [HttpGet("{id}/material")]
        [Authorize]
        public async Task<IActionResult> BaixarMaterial(int id)
        {
            var bytes = await _service.BaixarMaterialApoio(id);

            if (bytes == null)
                return NotFound();

            return File(bytes, "application/pdf", "material_apoio.pdf");
        }

        // =========================================================
        // 6. OBTER AULA POR ID
        // =========================================================

        /// <summary>
        /// Obtém uma aula específica pelo ID.
        ///
        /// Esse endpoint pode ser usado em telas de detalhe da aula,
        /// tanto na visão administrativa/professor quanto na visão do aluno.
        /// </summary>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> Obter(int id)
        {
            var aula = await _service.ObterPorId(id);

            return aula == null ? NotFound() : Ok(aula);
        }

        // =========================================================
        // 7. LISTAR AULAS POR TURMA/DISCIPLINA
        // =========================================================

        /// <summary>
        /// Lista as aulas cadastradas em uma TurmaDisciplina específica.
        ///
        /// Esse endpoint é usado para exibir as aulas de uma disciplina dentro
        /// de uma turma, que é o contexto acadêmico real do conteúdo.
        /// </summary>
        [HttpGet("turma-disciplina/{turmaDisciplinaId}")]
        [Authorize]
        public async Task<IActionResult> ListarPorTurmaDisciplina(int turmaDisciplinaId)
        {
            var lista = await _service.ListarPorTurmaDisciplina(turmaDisciplinaId);

            return Ok(lista);
        }

        // =========================================================
        // 8. LISTAR TODAS AS AULAS
        // =========================================================

        /// <summary>
        /// Lista todas as aulas cadastradas.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois representa
        /// uma visão administrativa geral do conteúdo cadastrado.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> Listar()
        {
            var lista = await _service.Listar();

            return Ok(lista);
        }

        // =========================================================
        // 9. DELETAR AULA
        // =========================================================

        /// <summary>
        /// Remove uma aula.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin no fluxo atual.
        /// Caso a aula não exista, retorna NotFound.
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "0,1")]
        public async Task<IActionResult> Deletar(int id)
        {
            var ok = await _service.Deletar(id);

            return ok ? NoContent() : NotFound();
        }

        // =========================================================
        // 10. ATUALIZAR AULA
        // =========================================================

        /// <summary>
        /// Atualiza os dados de uma aula existente.
        ///
        /// Esse endpoint é permitido para SuperAdmin, Admin e Professor.
        /// O ID do usuário autenticado é enviado ao Service para validação
        /// de permissão antes da atualização.
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "0,1,2")]
        public async Task<IActionResult> Atualizar(int id, AtualizarAulaDTO dto)
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);

            var aula = await _service.Atualizar(id, usuarioId, dto);

            if (aula == null)
                return NotFound("Aula não encontrada.");

            return Ok(aula);
        }

        // =========================================================
        // 11. MINHAS AULAS
        // =========================================================

        /// <summary>
        /// Lista as aulas disponíveis para o aluno logado.
        ///
        /// Esse endpoint é exclusivo para alunos. O Service localiza o aluno
        /// a partir do usuário autenticado, verifica sua matrícula ativa
        /// e retorna as aulas da turma correspondente.
        /// </summary>
        [HttpGet("minhas")]
        [Authorize(Roles = "3")]
        public async Task<IActionResult> ListarMinhasAulas()
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);

            var aulas = await _service.ListarMinhasAulas(usuarioId);

            return Ok(aulas);
        }
    }
}