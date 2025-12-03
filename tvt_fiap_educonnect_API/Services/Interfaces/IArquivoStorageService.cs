using Microsoft.AspNetCore.Http;

namespace EduConnect_API.Services.Interfaces
{
    public interface IArquivoStorageService
    {
        Task<string> SalvarAsync(IFormFile arquivo, string caminhoRelativo);
        bool ArquivoExiste(string caminhoRelativo);
        Task<byte[]> BaixarAsync(string caminhoRelativo);

        // Upload específico de entrega de atividade
        Task<string> SalvarEntrega(Guid atividadeId, Guid alunoId, IFormFile arquivo);
    }
}