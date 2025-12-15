using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAccountService
    {
        // CONTEXTO SELF 
        Task<MeContextoDTO> ObterContexto(Guid usuarioId, string tipoUsuario);

        // CONTEXTO ADMINISTRATIVO
        Task<MeContextoDTO> ObterContextoAluno(Guid alunoId);
        Task<MeContextoDTO> ObterContextoProfessor(Guid professorId);
    }
}
