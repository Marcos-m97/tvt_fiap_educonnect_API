using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IDisciplinaService
    {
        Task<DisciplinaDTO> Criar(CriarDisciplinaDTO dto);
        Task<IEnumerable<DisciplinaDTO>> Listar();
        Task<IEnumerable<DisciplinaDTO>> ListarPorCurso(int cursoId);
        Task<DisciplinaDTO?> ObterPorId(int id);
        Task<DisciplinaDTO?> Atualizar(int id, CriarDisciplinaDTO dto);
        Task<bool> Deletar(int id);
    }
}