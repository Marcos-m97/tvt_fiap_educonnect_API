using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IAlunoRepository
    {
        Task<Aluno> Criar(Aluno aluno);
        Task<Aluno?> ObterPorUsuarioId(int usuarioId);
        Task<Aluno?> ObterPorId(int id);
        Task<IEnumerable<Aluno>> Listar();
        Task<Aluno> Atualizar(Aluno aluno);
    }
}
