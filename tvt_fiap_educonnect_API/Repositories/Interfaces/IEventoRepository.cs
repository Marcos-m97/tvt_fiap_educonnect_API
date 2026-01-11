using EduConnect_API.Models;

namespace EduConnect_API.Repositories.Interfaces
{
    public interface IEventoRepository
    {
        Task<Evento> Criar(Evento evento);
        Task<Evento?> Obter(int id);
        Task<IEnumerable<Evento>> Listar();
        Task<IEnumerable<Evento>> ListarPorTurma(int turmaId);
        Task<IEnumerable<Evento>> ListarPorUsuario(int usuarioId);
        Task<Evento> Atualizar(Evento evento);
        Task<bool> Deletar(int id);
    }
}