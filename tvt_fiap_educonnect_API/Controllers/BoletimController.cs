using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados aos boletins acadêmicos.
    ///
    /// No EduConnect, o boletim consolida o desempenho de um aluno em uma turma,
    /// agrupando os resultados por disciplina e detalhando as atividades avaliadas.
    ///
    /// Este controller expõe operações para gerar boletim, consultar boletim,
    /// listar boletins por aluno, gerar PDF e visualizar uma prévia antes de salvar.
    /// </summary>
    [ApiController]
    [Route("api/boletins")]
    public class BoletimController : ControllerBase
    {
        private readonly IBoletimService _service;

        /// <summary>
        /// Recebe o serviço de boletins por injeção de dependência.
        ///
        /// O BoletimService concentra as regras de cálculo, geração,
        /// consulta e exportação em PDF do boletim.
        /// </summary>
        public BoletimController(IBoletimService service)
        {
            _service = service;
        }

        // ============================================================
        // 1. GERAR BOLETIM
        // ============================================================

        /// <summary>
        /// Gera e salva um novo boletim para um aluno em uma turma.
        ///
        /// O Service busca as disciplinas da turma, as atividades de cada disciplina
        /// e as entregas do aluno para calcular notas, médias e situação acadêmica.
        ///
        /// No fluxo atual, a geração cria um novo registro de boletim,
        /// sem sobrescrever boletins anteriores.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Gerar(CreateBoletimDTO dto)
        {
            var boletim = await _service.Gerar(dto);

            return Ok(boletim);
        }

        // ============================================================
        // 2. OBTER BOLETIM POR ID
        // ============================================================

        /// <summary>
        /// Obtém um boletim específico pelo ID.
        ///
        /// O retorno contém a estrutura consolidada do boletim,
        /// incluindo disciplinas e atividades relacionadas.
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> Obter(int id)
        {
            var result = await _service.Obter(id);

            return result == null ? NotFound() : Ok(result);
        }

        // ============================================================
        // 3. LISTAR BOLETINS POR ALUNO
        // ============================================================

        /// <summary>
        /// Lista todos os boletins vinculados a um aluno.
        ///
        /// Esse endpoint pode ser usado na visão do aluno ou em telas administrativas
        /// para consultar o histórico de boletins gerados.
        /// </summary>
        [HttpGet("aluno/{alunoId}")]
        public async Task<IActionResult> ListarPorAluno(int alunoId)
        {
            var boletins = await _service.ListarPorAluno(alunoId);

            return Ok(boletins);
        }

        // ============================================================
        // 4. GERAR PDF DO BOLETIM
        // ============================================================

        /// <summary>
        /// Gera o PDF de um boletim existente.
        ///
        /// O Service busca o boletim, os dados do aluno e da turma,
        /// e delega a criação do arquivo ao gerador de PDF.
        ///
        /// O Controller retorna o arquivo como application/pdf.
        /// </summary>
        [HttpGet("{id}/pdf")]
        public async Task<IActionResult> Pdf(int id)
        {
            var pdf = await _service.GerarPdf(id);

            return File(pdf, "application/pdf", $"boletim-{id}.pdf");
        }

        // ============================================================
        // 5. PREVIEW DO BOLETIM
        // ============================================================

        /// <summary>
        /// Gera uma prévia do boletim sem salvar no banco.
        ///
        /// Esse endpoint permite visualizar o resultado calculado antes
        /// de persistir oficialmente o boletim.
        ///
        /// A lógica de cálculo fica no Service e é semelhante à geração definitiva.
        /// </summary>
        [HttpGet("preview/{alunoId}/{turmaId}")]
        public async Task<IActionResult> Preview(int alunoId, int turmaId)
        {
            var preview = await _service.Preview(alunoId, turmaId);

            return Ok(preview);
        }
    }
}