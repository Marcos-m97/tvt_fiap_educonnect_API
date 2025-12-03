using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAlunoService
    {
        Task<AlunoDTO> Criar(CriarAlunoDTO dto);
        Task<AlunoDTO?> ObterPorUsuario(Guid usuarioId);
        Task<IEnumerable<AlunoDTO>> Listar();
        Task<AlunoDTO?> Atualizar(Guid id, CriarAlunoDTO dto);
    }
}