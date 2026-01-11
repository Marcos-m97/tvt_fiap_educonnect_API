

    using EduConnect_API.Models;
    namespace EduConnect_API.Repositories.Interfaces {

    public interface IUsuarioRepository
    {
        Task<Usuario?> ObterPorEmail(string email);
        Task<Usuario?> ObterPorId(int id);
        Task<Usuario> Criar(Usuario usuario);
        Task<IEnumerable<Usuario>> ListarTodos();
        Task<Usuario> Atualizar(Usuario usuario);
        Task<bool> SoftDelete(int id);
        Task<bool> Reativar(int id);

    
    }
}
