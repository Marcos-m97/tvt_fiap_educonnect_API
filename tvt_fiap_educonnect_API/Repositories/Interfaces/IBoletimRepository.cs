using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IBoletimRepository
    {
        Task<Boletim> Criar(Boletim boletim);
        Task<Boletim?> Obter(int id);
        Task<IEnumerable<Boletim>> ListarPorAluno(int alunoId);
    }
}

