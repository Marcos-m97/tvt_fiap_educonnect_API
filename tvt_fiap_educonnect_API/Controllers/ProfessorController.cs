using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados ao perfil docente.
    ///
    /// No EduConnect, o Professor é uma extensão da entidade Usuario.
    /// Enquanto Usuario concentra dados comuns de autenticação e autorização,
    /// Professor armazena dados específicos do perfil docente, como especialidade,
    /// formação, currículo Lattes e vínculos com turmas/disciplinas.
    ///
    /// Este controller recebe as requisições HTTP do frontend e delega as regras
    /// de negócio para o ProfessorService e para o AccountService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _service;
        private readonly IAccountService _accountService;

        /// <summary>
        /// Recebe as dependências necessárias por injeção de dependência.
        ///
        /// IProfessorService concentra as regras específicas do cadastro docente.
        /// IAccountService é utilizado para buscar informações consolidadas
        /// do professor dentro do contexto acadêmico da plataforma.
        /// </summary>
        public ProfessorController(IProfessorService service, IAccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }

        // ============================================================
        // 1. CRIAR PROFESSOR
        // ============================================================

        /// <summary>
        /// Cria o perfil docente de um professor.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin, pois a criação de professores
        /// deve ser controlada pela administração da plataforma.
        ///
        /// A regra de negócio principal fica no ProfessorService, que valida se
        /// o UsuarioId informado pertence a um usuário com Tipo = 2.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarProfessorDTO dto)
        {
            var professor = await _service.Criar(dto);

            return Ok(professor);
        }

        // ============================================================
        // 2. LISTAR PROFESSORES
        // ============================================================

        /// <summary>
        /// Lista todos os professores cadastrados.
        ///
        /// Esse endpoint pode ser acessado por perfis administrativos e professores.
        /// Ele é utilizado em telas de gestão acadêmica, associação de disciplinas
        /// e consultas relacionadas ao corpo docente.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var professores = await _service.Listar();

            return Ok(professores);
        }

        // ============================================================
        // 3. OBTER PROFESSOR POR USUÁRIO
        // ============================================================

        /// <summary>
        /// Obtém o perfil de professor a partir do ID do usuário.
        ///
        /// Esse endpoint é usado quando o sistema possui o UsuarioId,
        /// normalmente vindo do usuário autenticado, e precisa localizar
        /// o cadastro correspondente na tabela de professores.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Obter(int usuarioId)
        {
            var prof = await _service.ObterPorUsuario(usuarioId);

            if (prof == null)
                return NotFound();

            return Ok(prof);
        }

        // ============================================================
        // 4. ATUALIZAR PROFESSOR
        // ============================================================

        /// <summary>
        /// Atualiza os dados acadêmicos do professor.
        ///
        /// Esse endpoint permite alterar informações como especialidade,
        /// formação e currículo Lattes.
        ///
        /// A atualização é permitida para SuperAdmin, Admin e Professor,
        /// possibilitando manutenção administrativa e edição do próprio perfil docente.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarProfessorDTO dto)
        {
            var prof = await _service.Atualizar(id, dto);

            if (prof == null)
                return NotFound();

            return Ok(prof);
        }

        // ============================================================
        // 5. CONTEXTO ACADÊMICO DO PROFESSOR
        // ============================================================

        /// <summary>
        /// Obtém informações consolidadas do contexto acadêmico do professor.
        ///
        /// Esse endpoint pode ser usado para montar uma visão mais completa
        /// do professor dentro da plataforma, incluindo seus vínculos acadêmicos,
        /// disciplinas, turmas ou outras informações agregadas pelo AccountService.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet("{id}/contexto")]
        public async Task<IActionResult> Contexto(int id)
        {
            var contexto = await _accountService.ObterContextoProfessor(id);

            return Ok(contexto);
        }
    }
}