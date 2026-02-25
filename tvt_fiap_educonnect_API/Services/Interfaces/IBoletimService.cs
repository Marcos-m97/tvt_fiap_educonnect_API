using EduConnect_API.Models.DTOs;

namespace EduConnect_API.Services.Interfaces
{
    public interface IBoletimService
    {
        Task<BoletimDTO> Gerar(CreateBoletimDTO dto);
        Task<BoletimDTO?> Obter(int boletimId);
        Task<IEnumerable<BoletimDTO>> ListarPorAluno(int alunoId);
        Task<byte[]> GerarPdf(int boletimId);
        Task<BoletimDTO> Preview(int alunoId, int turmaId);
    }
}