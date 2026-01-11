using EduConnect_API.Models;
namespace EduConnect_API.Repositories.Interfaces
{
    public interface IAtividadeRepository
    {
        Task<Atividade> Criar(Atividade atividade);
        Task<Atividade?> ObterPorId(int id);
        Task<IEnumerable<Atividade>> ListarPorTurmaDisciplina(int turmaDisciplinaId);
        Task<IEnumerable<Atividade>> ListarPorTurma(int turmaId);

    }
}