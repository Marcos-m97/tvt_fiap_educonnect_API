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
            // Obtém o ID do usuário via Claim do JWT
            var idClaim = User.FindFirst("id")?.Value;

            if (idClaim == null)
                throw new AppException("Token inválido", 401);

            var id = Guid.Parse(idClaim);

            // Buscar usuário no banco
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
    }
}
