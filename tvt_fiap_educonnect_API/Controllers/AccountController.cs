using System.Security.Claims;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IAdminService _adminService;
        private readonly IProfessorService _professorService;
        private readonly IAlunoService _alunoService;

        public AccountController(
            IUsuarioService usuarioService,
            IAdminService adminService,
            IProfessorService professorService,
            IAlunoService alunoService)
        {
            _usuarioService = usuarioService;
            _adminService = adminService;
            _professorService = professorService;
            _alunoService = alunoService;
        }

        // ============================================================
        // GET /api/account/me  (Qualquer usuário logado)
        // ============================================================
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var userId = Guid.Parse(User.FindFirst("id")!.Value);
            var tipo = int.Parse(User.FindFirst(ClaimTypes.Role)!.Value);

            // Buscar dados básicos do usuário
            var usuario = await _usuarioService.ObterPorId(userId);
            if (usuario == null)
                return NotFound("Usuário não encontrado.");

            // Busca automática do perfil específico
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
                    usuario.Tipo
                },
                perfil
            });
        }
    }
}
