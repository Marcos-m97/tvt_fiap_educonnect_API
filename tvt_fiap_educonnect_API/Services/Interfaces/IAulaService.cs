using EduConnect_API.Models.DTOs;

public interface IAulaService
{
    Task<AulaDTO> Criar(Guid usuarioId, CriarAulaDTO dto);
    Task<AulaDTO?> ObterPorId(Guid id);
    Task<IEnumerable<AulaDTO>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId);
    Task<IEnumerable<AulaDTO>> ListarMinhasAulas(Guid usuarioId); // 👈 NOVO
    Task<IEnumerable<AulaDTO>> Listar();
    Task<AulaDTO?> UploadMaterialApoio(Guid aulaId, IFormFile arquivo);
    Task<byte[]?> BaixarMaterialApoio(Guid aulaId);
    Task<bool> Deletar(Guid id);
}
