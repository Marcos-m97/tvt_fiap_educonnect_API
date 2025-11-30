using Microsoft.AspNetCore.Mvc;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;

        public UsuarioController(IUsuarioService service)
        {
            _service = service;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO dto)
        {
            var user = await _service.Login(dto);

            if (user == null)
                return Unauthorized(new { message = "Credenciais inválidas" });

            return Ok(new
            {
                user.Id,
                user.Nome,
                user.Tipo
            });
        }
    }
}
