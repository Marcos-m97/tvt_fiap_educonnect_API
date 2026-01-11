using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IEntregaService
    {
        Task<EntregaDTO> CriarEntrega(int usuarioId, int atividadeId, IFormFile arquivo);
        Task<EntregaDTO> Corrigir(int entregaId, decimal nota, string? feedback);
        Task<IEnumerable<EntregaDTO>> ListarPorAtividade(int atividadeId);
        Task<IEnumerable<EntregaAlunoDTO>> ListarMinhasEntregas(int usuarioId,int? disciplinaId,int? atividadeId);
    }
}