using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface ITurmaDisciplinaRepository
    {
        Task<TurmaDisciplina> Criar(TurmaDisciplina entity);
        Task<TurmaDisciplina?> ObterPorId(int id);
        Task<IEnumerable<TurmaDisciplina>> Listar();
        Task<IEnumerable<TurmaDisciplina>> ListarPorTurma(int turmaId);
        Task<IEnumerable<TurmaDisciplina>> ListarPorProfessor(int professorId);
        Task<TurmaDisciplina> Atualizar(TurmaDisciplina entity);
        Task<bool> Deletar(int id);
    }
}