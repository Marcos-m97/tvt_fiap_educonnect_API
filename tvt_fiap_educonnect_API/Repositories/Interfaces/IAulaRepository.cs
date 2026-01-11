using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IAulaRepository
    {
        Task<Aula> Criar(Aula aula);
        Task<Aula?> ObterPorId(int id);

        Task<IEnumerable<Aula>> ListarPorTurmaDisciplina(int turmaDisciplinaId);

        // 🔹 NOVO: usado pelo painel do aluno
        Task<IEnumerable<Aula>> ListarPorTurma(int turmaId);

        Task<IEnumerable<Aula>> Listar();
        Task<Aula> Atualizar(Aula aula);
        Task<bool> Deletar(int id);
    }
}
