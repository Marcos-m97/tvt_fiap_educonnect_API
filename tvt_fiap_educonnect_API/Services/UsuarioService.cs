using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;

        public UsuarioService(IUsuarioRepository repo)
        {
            _repo = repo;
        }

        // ============================================================
        // 1. LOGIN (USADO NO CONTROLLER)
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
        // 2. OBTER POR ID (USADO NO /ME)
        // ============================================================
        public async Task<Usuario?> ObterPorId(Guid id)
        {
            return await _repo.ObterPorId(id);
        }

        // ============================================================
        // 3. CRIAR USUÁRIO (USADO NO POST /usuarios)
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
    }
}
