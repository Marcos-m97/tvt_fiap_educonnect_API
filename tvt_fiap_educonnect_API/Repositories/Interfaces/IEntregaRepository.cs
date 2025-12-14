using EduConnect_API.Models;
namespace EduConnect_API.Repositories.Interfaces
{
    public interface IEntregaRepository
    {
        Task<EntregaAtividade> Criar(EntregaAtividade entrega);
        Task<EntregaAtividade?> ObterPorId(Guid id);
        Task<IEnumerable<EntregaAtividade>> ListarPorAtividade(Guid atividadeId);
        Task<EntregaAtividade> Atualizar(EntregaAtividade entrega);
        Task<IEnumerable<EntregaAtividade>> ListarPorAluno(Guid alunoId);
    }
}