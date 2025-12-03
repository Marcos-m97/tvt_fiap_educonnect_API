using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> Criar(Aluno aluno);
        Task<Aluno?> ObterPorUsuarioId(Guid usuarioId);
        Task<Aluno?> ObterPorId(Guid id);
        Task<IEnumerable<Aluno>> Listar();
        Task<Aluno> Atualizar(Aluno aluno);
    }
}
