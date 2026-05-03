using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados às entregas de atividades.
    ///
    /// No EduConnect, a entrega representa a submissão de uma atividade feita
    /// por um aluno. Este controller organiza os fluxos de envio de atividade,
    /// correção pelo professor, listagem de entregas por atividade e consulta
    /// do histórico de entregas do aluno.
    ///
    /// As regras de negócio ficam concentradas no EntregaService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class EntregaController : ControllerBase
    {
        private readonly IEntregaService _service;

        /// <summary>
        /// Recebe o serviço de entregas por injeção de dependência.
        /// </summary>
        public EntregaController(IEntregaService service)
        {
            _service = service;
        }

        // ============================================================
        // 1. CRIAR ENTREGA
        // ============================================================

        /// <summary>
        /// Cria uma nova entrega de atividade para o aluno logado.
        ///
        /// Esse endpoint é exclusivo para alunos. O ID do usuário autenticado
        /// é recuperado pela claim "id" do token JWT e enviado ao Service.
        ///
        /// O Service usa esse ID para localizar o cadastro de aluno, validar
        /// a atividade e salvar o arquivo enviado.
        /// </summary>
        [Authorize(Roles = "3")]
        [HttpPost("{atividadeId}")]
        public async Task<IActionResult> Criar(int atividadeId, IFormFile arquivo)
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);

            var entrega = await _service.CriarEntrega(usuarioId, atividadeId, arquivo);

            return Ok(entrega);
        }

        // ============================================================
        // 2. CORRIGIR ENTREGA
        // ============================================================

        /// <summary>
        /// Corrige uma entrega de atividade.
        ///
        /// Esse endpoint é exclusivo para professores. Ele recebe a nota
        /// e o feedback enviados no corpo da requisição e delega a atualização
        /// ao EntregaService.
        /// </summary>
        [Authorize(Roles = "2")]
        [HttpPut("{entregaId}/corrigir")]
        public async Task<IActionResult> Corrigir(int entregaId, [FromBody] CorrigirDTO dto)
        {
            var entrega = await _service.Corrigir(
                entregaId,
                dto.Nota,
                dto.Feedback
            );

            return Ok(entrega);
        }

        // ============================================================
        // 3. LISTAR ENTREGAS POR ATIVIDADE
        // ============================================================

        /// <summary>
        /// Lista todas as entregas de uma atividade específica.
        ///
        /// Esse endpoint pode ser acessado por SuperAdmin, Admin e Professor.
        /// É usado principalmente na visão docente para acompanhar quais alunos
        /// entregaram uma atividade e quais entregas já foram corrigidas.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet("atividade/{atividadeId}")]
        public async Task<IActionResult> ListarPorAtividade(int atividadeId)
        {
            var entregas = await _service.ListarPorAtividade(atividadeId);

            return Ok(entregas);
        }

        // ============================================================
        // 4. LISTAR MINHAS ENTREGAS
        // ============================================================

        /// <summary>
        /// Lista o histórico de entregas do aluno logado.
        ///
        /// Esse endpoint é exclusivo para alunos. Ele permite filtros opcionais
        /// por disciplina e por atividade, facilitando a consulta de notas,
        /// feedbacks e arquivos enviados.
        ///
        /// O ID do usuário é obtido pelo token JWT e usado pelo Service para
        /// localizar o aluno correspondente.
        /// </summary>
        [Authorize(Roles = "3")]
        [HttpGet("minhas")]
        public async Task<IActionResult> ListarMinhasEntregas(
            [FromQuery] int? disciplinaId,
            [FromQuery] int? atividadeId)
        {
            var usuarioId = int.Parse(User.FindFirst("id")!.Value);

            var entregas = await _service.ListarMinhasEntregas(
                usuarioId,
                disciplinaId,
                atividadeId
            );

            return Ok(entregas);
        }
    }
}