using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IEntregaService
    {
        Task<EntregaDTO> CriarEntrega(Guid usuarioId, Guid atividadeId, IFormFile arquivo);
        Task<EntregaDTO> Corrigir(Guid entregaId, decimal nota, string? feedback);
        Task<IEnumerable<EntregaDTO>> ListarPorAtividade(Guid atividadeId);
    }
}