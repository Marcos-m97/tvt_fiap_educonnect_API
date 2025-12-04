using EduConnect_API.Models.DTOs;
using tvt_fiap_educonnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IEventoService
    {
        Task<EventoDTO> Criar(Guid criadorId, CriarEventoDTO dto);
        Task<EventoDTO?> Obter(Guid id);
        Task<IEnumerable<EventoDTO>> Listar();
        Task<IEnumerable<EventoDTO>> ListarPorTurma(Guid turmaId);
        Task<EventoDTO?> Atualizar(Guid id, CriarEventoDTO dto);
        Task<bool> Deletar(Guid id);
    }
}
