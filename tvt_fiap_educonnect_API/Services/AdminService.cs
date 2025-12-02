using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;
using EduConnect_API.Repositories;
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

        public async Task<Admin> Criar(CriarAdminDTO dto)
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

            return await _repo.Criar(admin);
        }

        public Task<Admin?> ObterPorUsuario(Guid usuarioId)
            => _repo.ObterPorUsuarioId(usuarioId);

        public Task<IEnumerable<Admin>> Listar()
            => _repo.Listar();

        public async Task<Admin?> Atualizar(Guid id, CriarAdminDTO dto)
        {
            var admin = await _repo.ObterPorId(id);

            if (admin == null)
                return null;

            admin.Departamento = dto.Departamento;
            admin.Cargo = dto.Cargo;

            return await _repo.Atualizar(admin);
        }
    }
}
