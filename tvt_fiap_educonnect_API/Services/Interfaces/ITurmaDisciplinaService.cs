using EduConnect_API.Models.DTOs;
using tvt_fiap_educonnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface ITurmaDisciplinaService
    {
        Task<TurmaDisciplinaDTO> Criar(CriarTurmaDisciplinaDTO dto);
        Task<TurmaDisciplinaDTO?> ObterPorId(int id);
        Task<IEnumerable<TurmaDisciplinaDTO>> Listar();
        Task<IEnumerable<TurmaDisciplinaDTO>> ListarPorTurma(int turmaId);
        Task<TurmaDisciplinaDTO?> Atualizar(int id, CriarTurmaDisciplinaDTO dto);
        Task<bool> Deletar(int id);
    }
}
