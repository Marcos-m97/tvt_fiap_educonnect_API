using System.Security.Claims;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints de conta do usuário autenticado.
    ///
    /// No EduConnect, este controller funciona como uma camada de apoio para o frontend
    /// recuperar informações do usuário logado e seu respectivo contexto de perfil.
    ///
    /// Diferente dos controllers específicos de Usuario, Aluno, Professor e Admin,
    /// este controller trabalha com o usuário autenticado a partir do token JWT.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAdminService _adminService;
        private readonly IProfessorService _professorService;
        private readonly IAlunoService _alunoService;
        private readonly IAccountService _accountService;

        /// <summary>
        /// Recebe os serviços necessários por injeção de dependência.
        ///
        /// IUsuarioService recupera os dados básicos do usuário.
        /// IAdminService, IProfessorService e IAlunoService recuperam o perfil específico.
        /// IAccountService monta contextos acadêmicos mais completos para cada tipo de usuário.
        /// </summary>
        public AccountController(
            IUsuarioService usuarioService,
            IAdminService adminService,
            IProfessorService professorService,
            IAlunoService alunoService,
            IAccountService accountService)
        {
            _usuarioService = usuarioService;
            _adminService = adminService;
            _professorService = professorService;
            _alunoService = alunoService;
            _accountService = accountService;
        }

        // ============================================================
        // 1. OBTER DADOS DO USUÁRIO LOGADO
        // ============================================================

        /// <summary>
        /// Retorna os dados básicos do usuário autenticado e seu perfil específico.
        ///
        /// O endpoint usa as claims do token JWT para identificar:
        /// - o ID do usuário;
        /// - o tipo/perfil do usuário.
        ///
        /// A partir do tipo, o sistema busca o perfil complementar:
        /// Admin, Professor ou Aluno.
        ///
        /// Esse endpoint é útil para o frontend montar informações de sessão,
        /// cabeçalho, menu lateral, dados de perfil e permissões de tela.
        /// </summary>
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var tipo = int.Parse(User.FindFirst(ClaimTypes.Role)!.Value);

            var usuario = await _usuarioService.ObterPorId(userId);

            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            var perfilTask = tipo switch
            {
                0 => _adminService.ObterPorUsuario(userId).ContinueWith(t => (object?)t.Result),
                1 => _adminService.ObterPorUsuario(userId).ContinueWith(t => (object?)t.Result),
                2 => _professorService.ObterPorUsuario(userId).ContinueWith(t => (object?)t.Result),
                3 => _alunoService.ObterPorUsuario(userId).ContinueWith(t => (object?)t.Result),
                _ => Task.FromResult<object?>(null)
            };

            var perfil = await perfilTask;

            return Ok(new
            {
                usuario = new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    usuario.Tipo,
                    usuario.FotoPerfilUrl
                },
                perfil
            });
        }

        // ============================================================
        // 2. OBTER CONTEXTO DO USUÁRIO LOGADO
        // ============================================================

        /// <summary>
        /// Retorna o contexto completo do usuário autenticado.
        ///
        /// Enquanto o endpoint /me retorna dados básicos do usuário e seu perfil,
        /// este endpoint delega ao AccountService a montagem de um contexto mais amplo.
        ///
        /// Exemplo:
        /// - para aluno, pode retornar matrícula ativa, turma, curso e dados acadêmicos;
        /// - para professor, pode retornar turmas e disciplinas vinculadas;
        /// - para admin, pode retornar informações administrativas relevantes.
        ///
        /// Esse endpoint ajuda o frontend a carregar a experiência correta
        /// conforme o perfil do usuário logado.
        /// </summary>
        [Authorize]
        [HttpGet("me/contexto")]
        public async Task<IActionResult> Contexto()
        {
            var userId = int.Parse(User.FindFirst("id")!.Value);
            var tipo = User.FindFirst(ClaimTypes.Role)!.Value;

            var contexto = await _accountService.ObterContexto(userId, tipo);

            if (contexto == null)
                return NotFound("Contexto não encontrado para o usuário.");

            return Ok(contexto);
        }
    }
}