using EduConnect_API.Models.DTOs;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AlunoController : ControllerBase
    {
        private readonly IAlunoService _service;
        private readonly IAccountService _accountService;


        public AlunoController(IAlunoService service, IAccountService accountService)
        {
            _service = service;
            _accountService = accountService;
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarAlunoDTO dto)
        {
            var aluno = await _service.Criar(dto);
            return Ok(aluno);
        }

        [Authorize(Roles = "0,1,2")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            return Ok(await _service.Listar());
        }

        [Authorize(Roles = "0,1,2,3")]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Obter(int usuarioId)
        {
            var aluno = await _service.ObterPorUsuario(usuarioId);
            if (aluno == null) return NotFound();

            return Ok(aluno);
        }

        [Authorize(Roles = "0,1,3")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarAlunoDTO dto)
        {
            var aluno = await _service.Atualizar(id, dto);
            if (aluno == null) return NotFound();

            return Ok(aluno);
        }
        [Authorize(Roles = "0,1,2")] // ADM e Professor
        [HttpGet("{id}/contexto")]
        public async Task<IActionResult> Contexto(int id)
        {
            return Ok(await _accountService.ObterContextoAluno(id));
        }

    }
}