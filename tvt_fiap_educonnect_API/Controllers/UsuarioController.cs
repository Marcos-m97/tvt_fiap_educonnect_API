using EduConnect_API.Exceptions;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    }
}
