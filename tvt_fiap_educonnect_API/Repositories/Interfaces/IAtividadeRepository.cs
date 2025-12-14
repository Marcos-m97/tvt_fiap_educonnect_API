using EduConnect_API.Models;
namespace EduConnect_API.Repositories.Interfaces
{
    public interface IAtividadeRepository
    {
        Task<Atividade> Criar(Atividade atividade);
        Task<Atividade?> ObterPorId(Guid id);
        Task<IEnumerable<Atividade>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId);
        Task<IEnumerable<Atividade>> ListarPorTurma(Guid turmaId);

    }
}