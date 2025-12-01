namespace EduConnect_API.Services.Interfaces
{
    using EduConnect_API.Models;
    using EduConnect_API.Models.DTOs;
    

    public interface IUsuarioService
    {
        Task<Usuario?> Login(LoginDTO dto);
        Task<Usuario?> ObterPorId(Guid id);
        Task<Usuario> Criar(CriarUsuarioDTO dto);
    }
}
