using EduConnect_API.Exceptions;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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

        // ============================================
        // 1. LOGIN (PÚBLICO)
        // ============================================
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _service.Login(dto);

            if (user == null)
                throw new AppException("Credenciais inválidas", 401);

            // Gerar JWT
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

        // ============================================
        // 2. /ME (USUÁRIO AUTENTICADO)
        // ============================================
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> Me()
        {
            var idClaim = User.FindFirst("id")?.Value;

            if (idClaim == null)
                throw new AppException("Token inválido", 401);

            var id = Guid.Parse(idClaim);
            var user = await _service.ObterPorId(id);

            if (user == null)
                throw new AppException("Usuário não encontrado", 404);

            return Ok(new
            {
                user.Id,
                user.Nome,
                user.Email,
                user.Tipo,
                user.CriadoEm
            });
        }

        // ============================================
        // 3. CRIAR USUÁRIO  (SUPERADMIN: 0 | ADMIN: 1)
        // ============================================
        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar([FromBody] CriarUsuarioDTO dto)
        {
            var tipoLogadoClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (tipoLogadoClaim == null)
                throw new AppException("Token inválido", 401);

            int tipoLogado = int.Parse(tipoLogadoClaim);

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
    }
}