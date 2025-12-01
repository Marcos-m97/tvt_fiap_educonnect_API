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
        public async Task<Usuario?> ObterPorId(Guid id)
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
                Id = Guid.NewGuid(),
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
        public async Task<Usuario?> Atualizar(Guid id, AtualizarUsuarioDTO dto)
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
        public async Task<bool> SoftDelete(Guid id)
        {
            return await _repo.SoftDelete(id);
        }

        // ============================================================
        // 7. REATIVAR
        // ============================================================
        public async Task<bool> Reativar(Guid id)
        {
            return await _repo.Reativar(id);
        }

        // ============================================================
        // 8. SOLICITAR RESET DE SENHA (envia código para email)
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

            await _emailService.EnviarEmail(
                email,
                "Código para Redefinição de Senha",
                $"Seu código para redefinição de senha é: {codigo}"
            );
        }

        // ============================================================
        // 9. RESETAR SENHA (verifica código + atualiza senha)
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
