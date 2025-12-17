using EduConnect_API.Models.DTOs;
using Microsoft.AspNetCore.Http;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAulaService
    {
        Task<AulaDTO> Criar(Guid usuarioId, CriarAulaDTO dto);
        Task<AulaDTO?> ObterPorId(Guid id);
        Task<IEnumerable<AulaDTO>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId);
        Task<IEnumerable<AulaDTO>> Listar();
        Task<AulaDTO?> UploadMaterialApoio(Guid aulaId, IFormFile arquivo);
        Task<byte[]?> BaixarMaterialApoio(Guid aulaId);
        Task<bool> Deletar(Guid id);
    }
}