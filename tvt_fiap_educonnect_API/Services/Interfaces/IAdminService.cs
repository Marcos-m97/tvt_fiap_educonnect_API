using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAdminService
    {
        Task<Admin> Criar(CriarAdminDTO dto);
        Task<Admin?> ObterPorUsuario(Guid usuarioId);
        Task<IEnumerable<Admin>> Listar();
        Task<Admin?> Atualizar(Guid id, CriarAdminDTO dto);
    }

}
