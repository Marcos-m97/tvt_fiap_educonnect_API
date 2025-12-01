using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> Login(LoginDTO dto);
        Task<Usuario?> ObterPorId(Guid id);
        Task<Usuario> Criar(CriarUsuarioDTO dto);
        Task<IEnumerable<Usuario>> ListarTodos();
        Task<Usuario?> Atualizar(Guid id, AtualizarUsuarioDTO dto);  
        Task<bool> SoftDelete(Guid id);
        Task<bool> Reativar(Guid id);

    }
}
