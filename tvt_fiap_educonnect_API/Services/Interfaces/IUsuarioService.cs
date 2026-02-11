using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> Login(LoginDTO dto);
        Task<Usuario?> ObterPorId(int id);
        Task<Usuario> Criar(CriarUsuarioDTO dto);
        //Task<IEnumerable<Usuario>> ListarTodos();
        Task<Usuario?> Atualizar(int id, AtualizarUsuarioDTO dto);  
        Task<bool> SoftDelete(int id);
        Task<bool> Reativar(int id);
        Task SolicitarResetSenha(string email);
        Task<bool> ResetarSenha(string email, string codigo, string novaSenha);
        Task<(IEnumerable<Usuario>, int)> ListarPaginado(int page, int pageSize, string? search);


    }
}
