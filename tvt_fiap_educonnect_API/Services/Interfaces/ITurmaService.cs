using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface ITurmaService
    {
        Task<TurmaDTO> Criar(CriarTurmaDTO dto);
        Task<TurmaDTO?> ObterPorId(Guid id);
        Task<IEnumerable<TurmaDTO>> Listar();
        Task<IEnumerable<TurmaDTO>> ListarPorCurso(Guid cursoId);
        Task<TurmaDTO?> Atualizar(Guid id, CriarTurmaDTO dto);
        Task<bool> Deletar(Guid id);
    }
}