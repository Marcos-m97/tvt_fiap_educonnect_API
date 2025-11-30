using EduConnect_API.Models;
using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Usuario?> Login(LoginDTO dto);
    }
}
