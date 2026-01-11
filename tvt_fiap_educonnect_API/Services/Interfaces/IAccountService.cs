using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAccountService
    {
        // CONTEXTO SELF 
        Task<MeContextoDTO> ObterContexto(int usuarioId, string tipoUsuario);

        // CONTEXTO ADMINISTRATIVO
        Task<MeContextoDTO> ObterContextoAluno(int alunoId);
        Task<MeContextoDTO> ObterContextoProfessor(int professorId);
    }
}
