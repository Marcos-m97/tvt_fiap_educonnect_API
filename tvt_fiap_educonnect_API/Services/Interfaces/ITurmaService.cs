using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface ITurmaService
    {
        Task<TurmaDTO> Criar(CriarTurmaDTO dto);
        Task<TurmaDTO?> ObterPorId(int id);
        Task<IEnumerable<TurmaDTO>> Listar();
        Task<IEnumerable<TurmaDTO>> ListarPorCurso(int cursoId);
        Task<TurmaDTO?> Atualizar(int id, CriarTurmaDTO dto);

        // 🔥 SOFT DELETE
        Task<bool> Deletar(int id);

        // 🔥 REATIVAR
        Task<bool> Reativar(int id);
    }
}