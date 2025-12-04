using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IBoletimService
    {
        Task<BoletimDTO> Gerar(CreateBoletimDTO dto);
        Task<BoletimDTO?> Obter(Guid boletimId);
        Task<IEnumerable<BoletimDTO>> ListarPorAluno(Guid alunoId);
        Task<byte[]> GerarPdf(Guid boletimId);
    }
}
