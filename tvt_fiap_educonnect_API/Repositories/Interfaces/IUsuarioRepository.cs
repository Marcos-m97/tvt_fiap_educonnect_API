using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmail(string email);
        Task<Usuario> Criar(Usuario usuario);
    }
}
