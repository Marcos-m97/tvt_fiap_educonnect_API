using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IEmailService _emailService;

        public UsuarioService(
            IUsuarioRepository repo,
            IPasswordResetRepository passwordResetRepository,
            IEmailService emailService)
        {
            _repo = repo;
            _passwordResetRepository = passwordResetRepository;
            _emailService = emailService;
        }

        // ============================================================
        // 1. LOGIN
        // ============================================================
        public async Task<Usuario?> Login(LoginDTO dto)
        {
            var user = await _repo.ObterPorEmail(dto.Email);
            if (user == null)
                return null;

            bool senhaValida = BCrypt.Net.BCrypt.Verify(dto.Senha, user.SenhaHash);
            if (!senhaValida)
                return null;

            return user;
        }

        // ============================================================
        // 2. OBTER POR ID (/me)
        // ============================================================
        public async Task<Usuario?> ObterPorId(int id)
        {
            return await _repo.ObterPorId(id);
        }

        // ============================================================
        // 3. CRIAR USUÁRIO
        // ============================================================
        public async Task<Usuario> Criar(CriarUsuarioDTO dto)
        {
            var novo = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                Tipo = dto.Tipo,
                CriadoEm = DateTime.UtcNow,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha)
            };

            return await _repo.Criar(novo);
        }

        // ============================================================
        // 4. LISTAR TODOS
        // ============================================================
        public async Task<IEnumerable<Usuario>> ListarTodos()
        {
            return await _repo.ListarTodos();
        }

        // ============================================================
        // 5. ATUALIZAR
        // ============================================================
        public async Task<Usuario?> Atualizar(int id, AtualizarUsuarioDTO dto)
        {
            var usuario = await _repo.ObterPorId(id);

            if (usuario == null || !usuario.Ativo)
                return null;

            usuario.Nome = dto.Nome;
            usuario.Email = dto.Email;
            usuario.Tipo = dto.Tipo;

            return await _repo.Atualizar(usuario);
        }

        // ============================================================
        // 6. SOFT DELETE
        // ============================================================
        public async Task<bool> SoftDelete(int id)
        {
            return await _repo.SoftDelete(id);
        }

        // ============================================================
        // 7. REATIVAR
        // ============================================================
        public async Task<bool> Reativar(int id)
        {
            return await _repo.Reativar(id);
        }

        // ============================================================
        // 8. SOLICITAR RESET DE SENHA (HTML + LINK)
        // ============================================================
        public async Task SolicitarResetSenha(string email)
        {
            var usuario = await _repo.ObterPorEmail(email);
            if (usuario == null) return;

            var codigo = new Random().Next(100000, 999999).ToString();

            var reset = new PasswordResetCode
            {
                Email = email,
                Codigo = codigo,
                ExpiraEm = DateTime.Now.AddMinutes(10),
                Usado = false
            };

            await _passwordResetRepository.Salvar(reset);
            var resetLinkComCodigo = $"http://localhost:5173/reset-password?email={email}&codigo={codigo}";

            var resetLinkManual = "http://localhost:5173/reset-password";

            var bodyHtml = $@"
                <div style='font-family: Arial, sans-serif; color: #333;'>
                    <h2>Redefinição de Senha</h2>

                    <p>Olá,</p>

                    <p>Recebemos uma solicitação para redefinir sua senha.</p>

                    <p>
                        <strong>Código de verificação:</strong><br/>
                        <span style='font-size: 20px; letter-spacing: 2px;'>
                            {codigo}
                        </span>
                    </p>

                    <p>
                        Você pode redefinir sua senha clicando no botão abaixo:
                    </p>

                    <p>
                        <a href='{resetLinkComCodigo}'
                           style='background-color:#4f46e5;
                                  color:white;
                                  padding:10px 16px;
                                  text-decoration:none;
                                  border-radius:6px;
                                  display:inline-block;'>
                            Redefinir Senha
                        </a>
                    </p>

                    <p style='margin-top:12px; font-size: 13px;'>
                        Caso o botão acima não funcione, acesse o link abaixo e informe o código manualmente:
                    </p>

                    <p>
                        <a href='{resetLinkManual}'
                           style='color:#4f46e5;'>
                            {resetLinkManual}
                        </a>
                    </p>

                    <p style='margin-top:20px; font-size: 12px; color: #666;'>
                        Este código expira em 10 minutos.<br/>
                        Se você não solicitou essa alteração, ignore este e-mail.
                    </p>
                </div>
                ";

            await _emailService.EnviarEmail(
                email,
                "Redefinição de Senha - EduConnect",
                bodyHtml,
                isHtml: true
            );
        }

        // ============================================================
        // 9. RESETAR SENHA
        // ============================================================
        public async Task<bool> ResetarSenha(string email, string codigo, string novaSenha)
        {
            var reset = await _passwordResetRepository.Obter(email, codigo);

            if (reset == null || reset.Usado || reset.ExpiraEm < DateTime.Now)
                return false;

            var usuario = await _repo.ObterPorEmail(email);
            if (usuario == null)
                return false;

            usuario.SenhaHash = BCrypt.Net.BCrypt.HashPassword(novaSenha);
            await _repo.Atualizar(usuario);

            reset.Usado = true;
            await _passwordResetRepository.Atualizar(reset);

            return true;
        }
    }
}
