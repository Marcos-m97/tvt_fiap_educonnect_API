namespace EduConnect_API.Repositories.Interfaces
{
    using EduConnect_API.Models;

    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmail(string email);
        Task<Usuario?> ObterPorId(Guid id);
        Task<Usuario> Criar(Usuario usuario);
    }
}
