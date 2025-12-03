using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface ICursoService
    {
        Task<CursoDTO> Criar(CriarCursoDTO dto);
        Task<IEnumerable<CursoDTO>> Listar();
        Task<CursoDTO?> ObterPorId(Guid id);
        Task<CursoDTO?> Atualizar(Guid id, CriarCursoDTO dto);
        Task<bool> Deletar(Guid id);
    }
}
