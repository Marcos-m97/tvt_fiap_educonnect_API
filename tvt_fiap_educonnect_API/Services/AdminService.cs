using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories.Interfaces;
using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _repo;
        private readonly IUsuarioRepository _usuarios;

        public AdminService(IAdminRepository repo, IUsuarioRepository usuarios)
        {
            _repo = repo;
            _usuarios = usuarios;
        }

        public async Task<AdminDTO> Criar(CriarAdminDTO dto)
        {
            var usuario = await _usuarios.ObterPorId(dto.UsuarioId);

            if (usuario == null || usuario.Tipo != 1)
                throw new Exception("Usuário não é um administrador.");

            var admin = new Admin
            {
                UsuarioId = dto.UsuarioId,
                Departamento = dto.Departamento,
                Cargo = dto.Cargo
            };

            admin = await _repo.Criar(admin);

            return MapToDTO(admin);
        }

        public async Task<AdminDTO?> ObterPorUsuario(Guid usuarioId)
        {
            var admin = await _repo.ObterPorUsuarioId(usuarioId);

            return admin == null ? null : MapToDTO(admin);
        }

        public async Task<IEnumerable<AdminDTO>> Listar()
        {
            var admins = await _repo.Listar();
            return admins.Select(a => MapToDTO(a));
        }

        public async Task<AdminDTO?> Atualizar(Guid id, CriarAdminDTO dto)
        {
            var admin = await _repo.ObterPorId(id);

            if (admin == null)
                return null;

            admin.Departamento = dto.Departamento;
            admin.Cargo = dto.Cargo;

            admin = await _repo.Atualizar(admin);

            return MapToDTO(admin);
        }

        private AdminDTO MapToDTO(Admin admin)
        {
            return new AdminDTO
            {
                Id = admin.Id,
                UsuarioId = admin.UsuarioId,
                Nome = admin.Usuario.Nome,
                Email = admin.Usuario.Email,
                Departamento = admin.Departamento,
                Cargo = admin.Cargo
            };
        }
    }
}
