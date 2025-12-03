using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IDisciplinaService
    {
        Task<DisciplinaDTO> Criar(CriarDisciplinaDTO dto);
        Task<IEnumerable<DisciplinaDTO>> Listar();
        Task<IEnumerable<DisciplinaDTO>> ListarPorCurso(Guid cursoId);
        Task<DisciplinaDTO?> ObterPorId(Guid id);
        Task<DisciplinaDTO?> Atualizar(Guid id, CriarDisciplinaDTO dto);
        Task<bool> Deletar(Guid id);
    }
}