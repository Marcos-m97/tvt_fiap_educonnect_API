using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IProfessorRepository
    {
        Task<Professor> Criar(Professor professor);
        Task<Professor?> ObterPorUsuarioId(int usuarioId);
        Task<Professor?> ObterPorId(int id);
        Task<IEnumerable<Professor>> Listar();
        Task<Professor> Atualizar(Professor professor);
    }
}
