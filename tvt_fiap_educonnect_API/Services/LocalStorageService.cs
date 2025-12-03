using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    public class LocalStorageService : IArquivoStorageService
    {
        private readonly IWebHostEnvironment _env;

        public LocalStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SalvarAsync(IFormFile arquivo, string caminhoRelativo)
        {
            var caminhoFisico = Path.Combine(_env.WebRootPath, caminhoRelativo);

            var pasta = Path.GetDirectoryName(caminhoFisico)!;
            Directory.CreateDirectory(pasta);

            using (var stream = new FileStream(caminhoFisico, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            // retorna caminho relativo para salvar no banco
            return "/" + caminhoRelativo.Replace("\\", "/");
        }

        public bool ArquivoExiste(string caminhoRelativo)
        {
            var caminhoFisico = Path.Combine(_env.WebRootPath, caminhoRelativo.TrimStart('/'));
            return File.Exists(caminhoFisico);
        }

        public async Task<byte[]> BaixarAsync(string caminhoRelativo)
        {
            var caminhoFisico = Path.Combine(_env.WebRootPath, caminhoRelativo.TrimStart('/'));

            if (!File.Exists(caminhoFisico))
                throw new FileNotFoundException();

            return await File.ReadAllBytesAsync(caminhoFisico);
        }
    }
}