using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ICursoRepository
    {
        Task<Curso> Criar(Curso curso);
        Task<Curso?> ObterPorId(int id);
        Task<IEnumerable<Curso>> Listar();
        Task<Curso> Atualizar(Curso curso);

        // 🔥 SOFT DELETE
        Task<bool> Deletar(int id);

        // 🔥 REATIVAR
        Task<bool> Reativar(int id);

        IQueryable<Curso> Query();
    }
}