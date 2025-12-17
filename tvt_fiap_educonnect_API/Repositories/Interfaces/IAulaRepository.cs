using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IAulaRepository
    {
        Task<Aula> Criar(Aula aula);
        Task<Aula?> ObterPorId(Guid id);
        Task<IEnumerable<Aula>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId);
        Task<IEnumerable<Aula>> Listar();
        Task<Aula> Atualizar(Aula aula);
        Task<bool> Deletar(Guid id);
    }
}