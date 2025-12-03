using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ITurmaRepository
    {
        Task<Turma> Criar(Turma turma);
        Task<Turma?> ObterPorId(Guid id);
        Task<IEnumerable<Turma>> Listar();
        Task<IEnumerable<Turma>> ListarPorCurso(Guid cursoId);
        Task<Turma> Atualizar(Turma turma);
        Task<bool> Deletar(Guid id);
    }
}