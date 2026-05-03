using System.Security.Claims;
using EduConnect_API.Exceptions;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Services;
using EduConnect_API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduConnect_API.Controllers
{
    /// <summary>
    /// Controller responsável pelos endpoints relacionados aos usuários do EduConnect.
    ///
    /// No sistema, Usuario é a entidade base de autenticação e autorização.
    /// A partir dela, o backend identifica o perfil de acesso do usuário:
    /// 0 = SuperAdmin
    /// 1 = Admin
    /// 2 = Professor
    /// 3 = Aluno
    ///
    /// Este controller concentra fluxos como login, criação administrativa de usuários,
    /// cadastro público de aluno, listagem, edição, soft delete, reativação,
    /// recuperação de senha e atualização de foto de perfil.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _service;
        private readonly JwtService _jwtService;

        /// <summary>
        /// Recebe as dependências necessárias por injeção de dependência.
        ///
        /// IUsuarioService concentra as regras de negócio relacionadas ao usuário.
        /// JwtService é responsável por gerar o token JWT após um login válido.
        /// </summary>
        public UsuarioController(IUsuarioService service, JwtService jwtService)
        {
            _service = service;
            _jwtService = jwtService;
        }

        // ============================================================
        // 1. LOGIN
        // ============================================================

        /// <summary>
        /// Realiza o login do usuário.
        ///
        /// Esse endpoint é público porque o usuário ainda não possui token.
        /// Ele recebe e-mail e senha, delega a validação ao UsuarioService
        /// e, caso as credenciais sejam válidas, gera um token JWT.
        ///
        /// O token retornado será usado pelo frontend nas próximas requisições
        /// para acessar endpoints protegidos de acordo com o perfil do usuário.
        /// </summary>
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
                    user.Tipo,
                    user.FotoPerfilUrl
                }
            });
        }

        // ============================================================
        // 2. CRIAR USUÁRIO
        // ============================================================

        /// <summary>
        /// Cria um novo usuário pelo painel administrativo.
        ///
        /// Esse endpoint é restrito a SuperAdmin e Admin.
        /// Além da autorização por role, existe uma regra adicional:
        /// usuários Admin não podem criar SuperAdmin ou outro Admin.
        ///
        /// Dessa forma, Admins comuns só podem criar professores e alunos,
        /// preservando a hierarquia de permissões da plataforma.
        /// </summary>
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
                    novo.CriadoEm,
                    novo.FotoPerfilUrl
                }
            });
        }

        // ============================================================
        // 3. REGISTRAR ALUNO
        // ============================================================

        /// <summary>
        /// Realiza o cadastro público de um novo aluno.
        ///
        /// Diferente da criação administrativa, esse endpoint é aberto
        /// para usuários que ainda não possuem conta.
        ///
        /// Por segurança, o Tipo é fixado como 3, garantindo que registros públicos
        /// sempre criem usuários com perfil de aluno, nunca Admin ou Professor.
        /// </summary>
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Registrar([FromBody] RegistrarAlunoDTO dto)
        {
            var novo = new CriarUsuarioDTO
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Senha = dto.Senha,
                Tipo = 3
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
                    usuario.Tipo,
                    usuario.FotoPerfilUrl
                }
            });
        }

        // ============================================================
        // 4. LISTAR USUÁRIOS
        // ============================================================

        /// <summary>
        /// Lista usuários com paginação e busca opcional.
        ///
        /// Esse endpoint é usado no painel administrativo para gerenciar usuários.
        /// A paginação evita carregar todos os registros de uma vez e o filtro
        /// de busca facilita localizar usuários por nome, e-mail ou ID.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 5,
            [FromQuery] string? search = null)
        {
            var (usuarios, total) = await _service.ListarPaginado(
                page,
                pageSize,
                search
            );

            return Ok(new
            {
                data = usuarios,
                total,
                page,
                pageSize
            });
        }

        // ============================================================
        // 5. OBTER USUÁRIO POR ID
        // ============================================================

        /// <summary>
        /// Obtém os dados de um usuário específico pelo ID.
        ///
        /// Esse endpoint é restrito a perfis administrativos e pode ser usado
        /// para consulta detalhada ou preenchimento de telas de edição.
        /// </summary>
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
        // 6. ATUALIZAR USUÁRIO
        // ============================================================

        /// <summary>
        /// Atualiza dados básicos de um usuário.
        ///
        /// Esse endpoint pode ser acessado por todos os perfis autenticados.
        /// No contexto do EduConnect, ele é usado tanto em fluxos administrativos
        /// quanto em edição de perfil.
        ///
        /// Uma melhoria futura seria reforçar regras específicas, por exemplo:
        /// garantir que aluno/professor só edite o próprio perfil e impedir
        /// promoção indevida de tipo de usuário.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Atualizar(int id, [FromBody] AtualizarUsuarioDTO dto)
        {
            var tipoLogadoClaim = User.FindFirst(ClaimTypes.Role)?.Value;

            if (tipoLogadoClaim == null)
                throw new AppException("Token inválido", 401);

            var atualizado = await _service.Atualizar(id, dto);

            if (atualizado == null)
                throw new AppException("Usuário não encontrado", 404);

            return Ok(atualizado);
        }

        // ============================================================
        // 7. SOFT DELETE
        // ============================================================

        /// <summary>
        /// Desativa logicamente um usuário.
        ///
        /// Em vez de remover fisicamente o registro do banco, o sistema altera
        /// o campo Ativo para false.
        ///
        /// Essa abordagem preserva histórico e relacionamentos acadêmicos,
        /// como matrículas, atividades, entregas e registros vinculados.
        /// </summary>
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
        // 8. REATIVAR USUÁRIO
        // ============================================================

        /// <summary>
        /// Reativa um usuário previamente desativado.
        ///
        /// Esse endpoint complementa o soft delete, permitindo restaurar o acesso
        /// de um usuário sem recriar seu cadastro.
        /// </summary>
        [Authorize(Roles = "0,1")]
        [HttpPut("{id}/reativar")]
        public async Task<IActionResult> Reativar(int id)
        {
            var sucesso = await _service.Reativar(id);

            if (!sucesso)
                throw new AppException("Usuário não encontrado", 404);

            return Ok(new { mensagem = "Usuário reativado com sucesso!" });
        }

        // ============================================================
        // 9. SOLICITAR RESET DE SENHA
        // ============================================================

        /// <summary>
        /// Solicita a recuperação de senha.
        ///
        /// O endpoint recebe o e-mail informado pelo usuário e delega ao Service
        /// a geração do código temporário e o envio do e-mail.
        ///
        /// A resposta é propositalmente genérica para não revelar se o e-mail
        /// existe ou não na base de usuários.
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDTO dto)
        {
            await _service.SolicitarResetSenha(dto.Email);

            return Ok(new { message = "Se o usuário existir, um e-mail foi enviado." });
        }

        // ============================================================
        // 10. RESETAR SENHA
        // ============================================================

        /// <summary>
        /// Redefine a senha do usuário a partir de um código de recuperação.
        ///
        /// O endpoint recebe e-mail, código e nova senha.
        /// A validação do código, expiração e atualização do hash da senha
        /// ficam concentradas no UsuarioService.
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO dto)
        {
            var result = await _service.ResetarSenha(
                dto.Email,
                dto.Codigo,
                dto.NovaSenha
            );

            if (!result)
                return BadRequest(new { message = "Código inválido ou expirado." });

            return Ok(new { message = "Senha redefinida com sucesso!" });
        }

        // ============================================================
        // 11. ATUALIZAR FOTO DE PERFIL
        // ============================================================

        /// <summary>
        /// Atualiza a foto de perfil de um usuário.
        ///
        /// O endpoint recebe um arquivo enviado pelo frontend e delega ao Service
        /// validações como extensão, tamanho, salvamento físico e atualização
        /// da URL da foto no cadastro do usuário.
        /// </summary>
        [Authorize(Roles = "0,1,2,3")]
        [HttpPut("{id}/foto")]
        public async Task<IActionResult> AtualizarFoto(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new AppException("Arquivo inválido", 400);

            var url = await _service.AtualizarFotoPerfil(id, file);

            if (url == null)
                throw new AppException("Usuário não encontrado", 404);

            return Ok(new { fotoUrl = url });
        }

        // ============================================================
        // 12. OBTER FOTO DE PERFIL
        // ============================================================

        /// <summary>
        /// Retorna a foto de perfil de um usuário como arquivo.
        ///
        /// O Service busca o caminho da imagem, lê os bytes do arquivo
        /// e informa o content type correto para que o Controller retorne
        /// a imagem usando File().
        ///
        /// Esse endpoint é público para facilitar a exibição da imagem em telas
        /// como login, perfil, listagens ou área acadêmica.
        /// </summary>
        [AllowAnonymous]
        [HttpGet("{id}/foto")]
        public async Task<IActionResult> ObterFoto(int id)
        {
            var result = await _service.ObterFotoPerfil(id);

            if (result == null)
                return NotFound();

            return File(result.Value.bytes, result.Value.contentType);
        }
    }
}