using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IDisciplinaRepository
    {
        Task<Disciplina> Criar(Disciplina disciplina);
        Task<IEnumerable<Disciplina>> Listar();
        Task<Disciplina?> ObterPorId(int id);
        Task<IEnumerable<Disciplina>> ListarPorCurso(int cursoId);
        Task<Disciplina> Atualizar(Disciplina disciplina);
        Task<bool> Deletar(int id);
    }
}