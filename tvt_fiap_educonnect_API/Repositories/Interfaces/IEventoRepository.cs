using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IEventoRepository
    {
        Task<Evento> Criar(Evento evento);
        Task<Evento?> Obter(Guid id);
        Task<IEnumerable<Evento>> Listar();
        Task<IEnumerable<Evento>> ListarPorTurma(Guid turmaId);
        Task<IEnumerable<Evento>> ListarPorUsuario(Guid usuarioId);
        Task<Evento> Atualizar(Evento evento);
        Task<bool> Deletar(Guid id);
    }
}