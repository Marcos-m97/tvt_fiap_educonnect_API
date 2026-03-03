using System.Security.Claims;
using EduConnect_API.Exceptions;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using tvt_fiap_educonnect_API.Models.DTOs;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly JwtService _jwtService;

        public UsuarioController(IUsuarioService service, JwtService jwtService)
        {
            _service = service;
            _jwtService = jwtService;
        }

        // ============================================================
        // 1. LOGIN (PÚBLICO)
        // ============================================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _service.Login(dto);

            if (user == null)
                throw new AppException("Credenciais inválidas", 401);

            var token = _jwtService.GenerateToken(user.Id, user.Nome, user.Tipo);

            return Ok(new
            {
                token,
                usuario = new
                {
                    user.Id,
                    user.Nome,
                    user.Tipo
                }
            });
        }

        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarUsuarioDTO dto)
        {
            var tipoLogadoClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (tipoLogadoClaim == null)
                throw new AppException("Token inválido", 401);

            int tipoLogado = int.Parse(tipoLogadoClaim);

            // ADMIN (1) não pode criar SUPERADMIN (0) e nem ADMIN (1)
            if (tipoLogado == 1 && (dto.Tipo == 0 || dto.Tipo == 1))
                throw new AppException("Admins só podem criar professores (2) e alunos (3).", 403);

            var novo = await _service.Criar(dto);

            return Ok(new
            {
                mensagem = "Usuário criado com sucesso!",
                usuario = new
                {
                    novo.Id,
                    novo.Nome,
                    novo.Email,
                    novo.Tipo,
                    novo.CriadoEm
                }
            });
        }

        // ============================================================
        // 4. REGISTRAR USUÁRIO (PÚBLICO)
        // ============================================================
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarAlunoDTO dto)
        {
            var novo = new CriarUsuarioDTO
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                Tipo = 3 // aluno
            };

            var usuario = await _service.Criar(novo);

            return Ok(new
            {
                mensagem = "Conta criada com sucesso!",
                usuario = new
                {
                    usuario.Id,
                    usuario.Nome,
                    usuario.Email,
                    usuario.Tipo
                }
            });
        }


        // ============================================================
        // 5. LISTAR PAGINADO (SUPERADMIN = 0 | ADMIN = 1)
        // ============================================================
        [Authorize(Roles = "0,1")]
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            var (usuarios, total) = await _service.ListarPaginado(page, pageSize, search);

            return Ok(new
            {
                data = usuarios,
                total,
                page,
                pageSize
            });
        }


        // ============================================================
        // 6. OBTER POR ID (SUPERADMIN = 0 | ADMIN = 1)
        // ============================================================
        [Authorize(Roles = "0,1")]
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(int id)
        {
            var usuario = await _service.ObterPorId(id);

            if (usuario == null)
                throw new AppException("Usuário não encontrado", 404);

            return Ok(usuario);
        }

        // ============================================================
        // 7. ATUALIZAR (SUPERADMIN = 0 | ADMIN = 1)
        // ============================================================
        [Authorize(Roles = "0,1,2,3")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDTO dto)
        {
            var tipoLogadoClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (tipoLogadoClaim == null)
                throw new AppException("Token inválido", 401);

            int tipoLogado = int.Parse(tipoLogadoClaim);

            // Admin não pode promover usuários acima dele
            if (tipoLogado == 1 && (dto.Tipo == 0 || dto.Tipo == 1))
                throw new AppException("Admins só podem editar professores (2) e alunos (3).", 403);

            var atualizado = await _service.Atualizar(id, dto);

            if (atualizado == null)
                throw new AppException("Usuário não encontrado", 404);

            return Ok(atualizado);
        }

        // ============================================================
        // 8. SOFT DELETE (SUPERADMIN = 0 | ADMIN = 1)
        // ============================================================
        [Authorize(Roles = "0,1")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var sucesso = await _service.SoftDelete(id);

            if (!sucesso)
                throw new AppException("Usuário não encontrado", 404);

            return NoContent();
        }
 
        // ============================================================
        // 9. REATIVAR USUARIOS (SUPERADMIN = 0 | ADMIN = 1)
        // ============================================================
        [Authorize(Roles = "0,1")] // SuperAdmin(0) ou Admin(1)
        [HttpPut("{id}/reativar")]
        public async Task<IActionResult> Reativar(int id)
        {
            var sucesso = await _service.Reativar(id);

            if (!sucesso)
                throw new AppException("Usuário não encontrado", 404);

            return Ok(new { mensagem = "Usuário reativado com sucesso!" });
        }

        // ============================================================
        // 10. SOLICITAR REDEFINIÇÃO DE SENHA
        // ============================================================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO dto)
        {
            await _service.SolicitarResetSenha(dto.Email);
            return Ok(new { message = "Se o usuário existir, um e-mail foi enviado." });
        }

        // ============================================================
        // 11. REDEFINIR DE SENHA
        // ============================================================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _service.ResetarSenha(dto.Email, dto.Codigo, dto.NovaSenha);

            if (!result)
                return BadRequest(new { message = "Código inválido ou expirado." });

            return Ok(new { message = "Senha redefinida com sucesso!" });
        }

    }
}