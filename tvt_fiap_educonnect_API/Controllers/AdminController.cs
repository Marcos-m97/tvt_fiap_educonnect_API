using EduConnect_API.Models.DTOs;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados ao perfil administrativo.
    ///
    /// No EduConnect, o Admin é uma extensão da entidade Usuario e representa
    /// usuários responsáveis por ações de gestão acadêmica e operacional,
    /// como gerenciamento de usuários, cursos, turmas, disciplinas e matrículas.
    ///
    /// Este controller recebe as requisições HTTP do frontend e delega as regras
    /// de negócio para o AdminService.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _service;

        /// <summary>
        /// Recebe o serviço de administradores por injeção de dependência.
        ///
        /// O AdminService concentra as regras de negócio, enquanto o controller
        /// organiza as rotas, permissões e respostas HTTP.
        /// </summary>
        public AdminController(IAdminService service)
        {
            _service = service;
        }

        // ============================================================
        // 1. CRIAR ADMIN
        // ============================================================

        /// <summary>
        /// Cria um novo perfil administrativo.
        ///
        /// Esse endpoint é restrito ao SuperAdmin, pois a criação de administradores
        /// é uma permissão sensível dentro da plataforma.
        ///
        /// A regra de negócio principal fica no AdminService, que valida se o usuário
        /// informado existe e possui Tipo = 1.
        /// </summary>
        [Authorize(Roles = "0")]
        [HttpPost]
        public async Task<IActionResult> Criar(CriarAdminDTO dto)
        {
            var admin = await _service.Criar(dto);

            return Ok(admin);
        }

        // ============================================================
        // 2. LISTAR ADMINS
        // ============================================================

        /// <summary>
        /// Lista todos os administradores cadastrados.
        ///
        /// Esse endpoint pode ser acessado por SuperAdmin e Admin,
        /// permitindo visualizar os perfis administrativos existentes.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var admins = await _service.Listar();

            return Ok(admins);
        }

        // ============================================================
        // 3. OBTER ADMIN POR USUÁRIO
        // ============================================================

        /// <summary>
        /// Obtém o perfil administrativo a partir do ID do usuário.
        ///
        /// Esse endpoint é útil quando o frontend possui o UsuarioId
        /// e precisa localizar o cadastro correspondente na tabela de Admin.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpGet("{usuarioId}")]
        public async Task<IActionResult> Obter(int usuarioId)
        {
            var admin = await _service.ObterPorUsuario(usuarioId);

            if (admin == null)
                return NotFound();

            return Ok(admin);
        }

        // ============================================================
        // 4. ATUALIZAR ADMIN
        // ============================================================

        /// <summary>
        /// Atualiza os dados específicos de um perfil administrativo.
        ///
        /// Esse endpoint permite alterar informações como departamento e cargo.
        /// Dados comuns como nome, e-mail e tipo ficam na entidade Usuario
        /// e são tratados no fluxo de usuários.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, CriarAdminDTO dto)
        {
            var admin = await _service.Atualizar(id, dto);

            if (admin == null)
                return NotFound();

            return Ok(admin);
        }
    }
}