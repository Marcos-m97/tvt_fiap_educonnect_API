namespace EduConnect_API.Services.Interfaces
{
    public interface IArquivoStorageService
    {
        Task<string> SalvarAsync(IFormFile arquivo, string caminhoRelativo);
        Task<byte[]> BaixarAsync(string caminhoRelativo);
        bool ArquivoExiste(string caminhoRelativo);
    }
}