using EduConnect_API.Models.DTOs;
using tvt_fiap_educonnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IEventoService
    {
        Task<EventoDTO> Criar(int criadorId, CriarEventoDTO dto);
        Task<EventoDTO?> Obter(int id);
        Task<IEnumerable<EventoDTO>> Listar();
        Task<IEnumerable<EventoDTO>> ListarPorTurma(int turmaId);
        Task<EventoDTO?> Atualizar(int id, CriarEventoDTO dto);
        Task<IEnumerable<EventoDTO>> ListarMeusEventos(int usuarioId);

        Task<bool> Deletar(int id);
    }
}