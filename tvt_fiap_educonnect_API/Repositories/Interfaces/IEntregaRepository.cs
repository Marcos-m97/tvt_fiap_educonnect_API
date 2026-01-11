using EduConnect_API.Models;
namespace EduConnect_API.Repositories.Interfaces
{
    public interface IEntregaRepository
    {
        Task<EntregaAtividade> Criar(EntregaAtividade entrega);
        Task<EntregaAtividade?> ObterPorId(int id);
        Task<IEnumerable<EntregaAtividade>> ListarPorAtividade(int atividadeId);
        Task<EntregaAtividade> Atualizar(EntregaAtividade entrega);
        Task<IEnumerable<EntregaAtividade>> ListarPorAluno(int alunoId);
    }
}