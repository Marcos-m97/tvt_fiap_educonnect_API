using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IBoletimRepository
    {
        Task<Boletim> Criar(Boletim boletim);
        Task<Boletim?> Obter(Guid id);
        Task<IEnumerable<Boletim>> ListarPorAluno(Guid alunoId);
    }
}

