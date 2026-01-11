using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface ICursoService
    {
        Task<CursoDTO> Criar(CriarCursoDTO dto);
        Task<IEnumerable<CursoDTO>> Listar();
        Task<CursoDTO?> ObterPorId(int id);
        Task<CursoDTO?> Atualizar(int id, CriarCursoDTO dto);
        Task<bool> Deletar(int id);
    }
}
