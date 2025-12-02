using EduConnect_API.Models;
namespace EduConnect_API.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<Admin> Criar(Admin admin);
        Task<Admin?> ObterPorUsuarioId(Guid usuarioId);
        Task<Admin?> ObterPorId(Guid id);
        Task<IEnumerable<Admin>> Listar();
        Task<Admin> Atualizar(Admin admin);
    }

}