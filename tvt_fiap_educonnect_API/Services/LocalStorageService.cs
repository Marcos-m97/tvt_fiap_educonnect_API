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
        // ENTREGAS DE ATIVIDADES
        public async Task<string> SalvarEntrega(int atividadeId, int alunoId, IFormFile arquivo)
        {
            var pasta = Path.Combine("wwwroot", "uploads", "entregas",
                atividadeId.ToString(),
                alunoId.ToString());

            Directory.CreateDirectory(pasta);

            var caminho = Path.Combine(pasta, arquivo.FileName);

            using var stream = new FileStream(caminho, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            // retorna caminho relativo
            return caminho.Replace("wwwroot", "").Replace("\\", "/");
        }


    }
}