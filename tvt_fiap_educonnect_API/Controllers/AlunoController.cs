using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados ao perfil acadêmico do aluno.
    ///
    /// No EduConnect, a entidade Aluno funciona como uma extensão da entidade Usuario.
    /// Enquanto Usuario concentra dados de autenticação e perfil de acesso,
    /// Aluno armazena informações acadêmicas e pessoais complementares, como CPF,
    /// data de nascimento, endereço, matrículas e entregas de atividades.
    ///
    /// Este controller recebe as requisições HTTP do frontend e delega as regras
    /// de negócio para o AlunoService e para o AccountService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _service;
        private readonly IAccountService _accountService;

        /// <summary>
        /// Recebe as dependências necessárias por injeção de dependência.
        ///
        /// IAlunoService concentra as regras específicas do cadastro acadêmico do aluno.
        /// IAccountService é utilizado para buscar informações consolidadas do aluno
        /// dentro do contexto acadêmico da plataforma.
        /// </summary>
        public AlunoController(IAlunoService service, IAccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }

        // ============================================================
        // 1. CRIAR ALUNO
        // ============================================================

        /// <summary>
        /// Cria o perfil acadêmico de um aluno.
        ///
        /// Esse endpoint vincula dados específicos de aluno a um usuário já existente.
        /// A regra de negócio principal fica no AlunoService, que valida se o UsuarioId
        /// informado pertence a um usuário com Tipo = 3, ou seja, perfil de aluno.
        ///
        /// No fluxo do EduConnect, esse cadastro complementa a conta do usuário
        /// e prepara o aluno para seguir com o processo de matrícula.
        /// </summary>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarAlunoDTO dto)
        {
            var aluno = await _service.Criar(dto);

            return Ok(aluno);
        }

        // ============================================================
        // 2. LISTAR ALUNOS
        // ============================================================

        /// <summary>
        /// Lista todos os alunos cadastrados.
        ///
        /// Esse endpoint é restrito a perfis administrativos e professores.
        /// Ele permite que usuários autorizados consultem a base de alunos,
        /// por exemplo em telas de gestão acadêmica, acompanhamento ou associação
        /// com turmas e disciplinas.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var alunos = await _service.Listar();

            return Ok(alunos);
        }

        // ============================================================
        // 3. OBTER ALUNO POR USUÁRIO
        // ============================================================

        /// <summary>
        /// Obtém o perfil de aluno a partir do ID do usuário.
        ///
        /// Esse endpoint é usado quando o sistema possui o UsuarioId, normalmente
        /// vindo do usuário autenticado, e precisa localizar o cadastro acadêmico
        /// correspondente na tabela de alunos.
        ///
        /// Todos os perfis autenticados podem acessar esse endpoint porque ele é útil
        /// tanto para administração quanto para o próprio aluno consultar seus dados.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Obter(int usuarioId)
        {
            var aluno = await _service.ObterPorUsuario(usuarioId);

            if (aluno == null)
                return NotFound();

            return Ok(aluno);
        }

        // ============================================================
        // 4. ATUALIZAR ALUNO
        // ============================================================

        /// <summary>
        /// Atualiza os dados pessoais do aluno.
        ///
        /// Esse endpoint permite alterar informações complementares do cadastro,
        /// como CPF, data de nascimento e endereço.
        ///
        /// A atualização é permitida para SuperAdmin, Admin e Aluno. No contexto
        /// do portal, isso permite tanto manutenção administrativa quanto edição
        /// do próprio cadastro pelo aluno.
        /// </summary>
        [Authorize(Roles = "0,1,3")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarAlunoDTO dto)
        {
            var aluno = await _service.Atualizar(id, dto);

            if (aluno == null)
                return NotFound();

            return Ok(aluno);
        }

        // ============================================================
        // 5. CONTEXTO ACADÊMICO DO ALUNO
        // ============================================================

        /// <summary>
        /// Obtém informações consolidadas do contexto acadêmico do aluno.
        ///
        /// Esse endpoint é utilizado por perfis administrativos e professores
        /// para consultar uma visão mais completa do aluno dentro da plataforma.
        ///
        /// A responsabilidade de montar esse contexto fica no AccountService,
        /// que pode consolidar informações relacionadas ao aluno, como vínculos,
        /// matrículas, turmas, disciplinas ou outros dados acadêmicos necessários
        /// para a tela de acompanhamento.
        /// </summary>
        [Authorize(Roles = "0,1,2")]
        [HttpGet("{id}/contexto")]
        public async Task<IActionResult> Contexto(int id)
        {
            var contexto = await _accountService.ObterContextoAluno(id);

            return Ok(contexto);
        }
    }
}