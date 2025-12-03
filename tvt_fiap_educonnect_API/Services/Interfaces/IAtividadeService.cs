using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IAtividadeService
    {
        Task<AtividadeDTO> Criar(CriarAtividadeDTO dto);
        Task<IEnumerable<AtividadeDTO>> ListarPorTurmaDisciplina(Guid turmaDisciplinaId);
    }
}

