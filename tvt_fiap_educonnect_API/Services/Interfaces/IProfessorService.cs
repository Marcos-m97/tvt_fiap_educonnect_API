using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IProfessorService
    {
        Task<ProfessorDTO> Criar(CriarProfessorDTO dto);
        Task<ProfessorDTO?> ObterPorUsuario(Guid usuarioId);
        Task<IEnumerable<ProfessorDTO>> Listar();
        Task<ProfessorDTO?> Atualizar(Guid id, CriarProfessorDTO dto);
    }
}
