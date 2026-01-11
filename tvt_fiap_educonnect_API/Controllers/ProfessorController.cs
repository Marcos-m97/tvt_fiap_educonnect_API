using EduConnect_API.Models.DTOs;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProfessorController : ControllerBase
    {
        private readonly IProfessorService _service;
        private readonly IAccountService _accountService;

        public ProfessorController(IProfessorService service, IAccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }

        [Authorize(Roles = "0,1")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarProfessorDTO dto)
        {
            var professor = await _service.Criar(dto);
            return Ok(professor);
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Obter(int usuarioId)
        {
            var prof = await _service.ObterPorUsuario(usuarioId);
            if (prof == null) return NotFound();

            return Ok(prof);
        }

        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarProfessorDTO dto)
        {
            var prof = await _service.Atualizar(id, dto);
            if (prof == null) return NotFound();

            return Ok(prof);
        }
        [Authorize(Roles = "0,1")] // apenas ADM
        [HttpGet("{id}/contexto")]
        public async Task<IActionResult> Contexto(int id)
        {
            return Ok(await _accountService.ObterContextoProfessor(id));
        }

    }
}
