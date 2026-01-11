using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IMatriculaRepository
    {
        Task<Matricula> Criar(Matricula matricula);
        Task<Matricula?> ObterPorId(int id);
        Task<IEnumerable<Matricula>> Listar();
        Task<IEnumerable<Matricula>> ListarPorAluno(int alunoId);
        Task<IEnumerable<Matricula>> ListarPorTurma(int turmaId);
        Task<Matricula> Atualizar(Matricula matricula);
        Task<bool> Deletar(int id);
        Task<Matricula?> ObterAtivaPorAlunoId(int alunoId);

    }
}