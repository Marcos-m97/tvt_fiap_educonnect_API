using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ITurmaDisciplinaRepository
    {
        Task<TurmaDisciplina> Criar(TurmaDisciplina entity);
        Task<TurmaDisciplina?> ObterPorId(Guid id);
        Task<IEnumerable<TurmaDisciplina>> Listar();
        Task<IEnumerable<TurmaDisciplina>> ListarPorTurma(Guid turmaId);
        Task<TurmaDisciplina> Atualizar(TurmaDisciplina entity);
        Task<bool> Deletar(Guid id);
    }
}