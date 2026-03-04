using EduConnect_API.Models.DTOs;

public interface IAulaService
{
    Task<AulaDTO> Criar(int usuarioId, CriarAulaDTO dto);

    Task<AulaDTO?> ObterPorId(int id);

    Task<IEnumerable<AulaDTO>> ListarPorTurmaDisciplina(int turmaDisciplinaId);

    Task<IEnumerable<AulaDTO>> ListarMinhasAulas(int usuarioId); // aulas do aluno

    Task<IEnumerable<AulaDTO>> Listar();

    Task<AulaDTO?> Atualizar(int aulaId, int usuarioId, AtualizarAulaDTO dto);

    // ============================
    // MATERIAL DE APOIO
    // ============================
    Task<AulaDTO?> UploadMaterialApoio(int aulaId, IFormFile arquivo);

    Task<byte[]?> BaixarMaterialApoio(int aulaId);

    // ============================
    // VIDEO DA AULA (MP4)
    // ============================
    Task<AulaDTO?> UploadVideoAula(int aulaId, IFormFile arquivo);

    Task<bool> Deletar(int id);
}