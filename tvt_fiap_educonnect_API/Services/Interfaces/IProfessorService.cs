using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IProfessorService
    {
        Task<ProfessorDTO> Criar(CriarProfessorDTO dto);
        Task<ProfessorDTO?> ObterPorUsuario(int usuarioId);
        Task<IEnumerable<ProfessorDTO>> Listar();
        Task<ProfessorDTO?> Atualizar(int id, CriarProfessorDTO dto);
    }
}
