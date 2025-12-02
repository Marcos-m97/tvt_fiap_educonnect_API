using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        public AdminController(IAdminService service)
        {
            _service = service;
        }

        [Authorize(Roles = "0")] // superadmin
        [HttpPost]
        public async Task<IActionResult> Criar(CriarAdminDTO dto)
        {
            var admin = await _service.Criar(dto);
            return Ok(admin);
        }

        [Authorize(Roles = "0,1")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [Authorize(Roles = "0,1")]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Obter(Guid usuarioId)
        {
            var admin = await _service.ObterPorUsuario(usuarioId);
            if (admin == null) return NotFound();

            return Ok(admin);
        }

        [Authorize(Roles = "0")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(Guid id, CriarAdminDTO dto)
        {
            var admin = await _service.Atualizar(id, dto);
            if (admin == null) return NotFound();

            return Ok(admin);
        }
    }
}