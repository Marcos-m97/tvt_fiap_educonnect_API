using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IMatriculaRepository
    {
        Task<Matricula> Criar(Matricula matricula);
        Task<Matricula?> ObterPorId(Guid id);
        Task<IEnumerable<Matricula>> Listar();
        Task<IEnumerable<Matricula>> ListarPorAluno(Guid alunoId);
        Task<IEnumerable<Matricula>> ListarPorTurma(Guid turmaId);
        Task<Matricula> Atualizar(Matricula matricula);
        Task<bool> Deletar(Guid id);
        Task<Matricula?> ObterAtivaPorAlunoId(Guid alunoId);

    }
}