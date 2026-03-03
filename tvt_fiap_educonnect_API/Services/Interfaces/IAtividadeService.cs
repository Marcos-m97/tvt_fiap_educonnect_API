using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAtividadeService
    {
        Task<AtividadeDTO> Criar(CriarAtividadeDTO dto);
        Task<IEnumerable<AtividadeDTO>> ListarPorTurmaDisciplina(int turmaDisciplinaId);
        Task<IEnumerable<AtividadeAlunoDTO>> ListarMinhasAtividades(int usuarioId);
        Task<AtividadeDTO> Atualizar(int id, AtualizarAtividadeDTO dto);
        Task<AtividadeDTO> ObterPorId(int id);

    } 
}

