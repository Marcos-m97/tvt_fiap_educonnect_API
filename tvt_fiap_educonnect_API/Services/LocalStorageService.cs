using EduConnect_API.Services.Interfaces;

namespace EduConnect_API.Services
{
    /// <summary>
    /// Serviço responsável por salvar, localizar e baixar arquivos no armazenamento local da aplicação.
    ///
    /// No EduConnect, esse serviço é usado para arquivos como:
    /// - materiais de apoio das aulas;
    /// - vídeos de aula;
    /// - documentos de matrícula;
    /// - comprovantes de pagamento;
    /// - entregas de atividades dos alunos.
    ///
    /// A centralização do armazenamento em um serviço permite que outras camadas,
    /// como Services de Aula, Matrícula e Entrega, não precisem conhecer detalhes
    /// de caminho físico, wwwroot ou manipulação direta de arquivos.
    /// </summary>
    public class LocalStorageService : IArquivoStorageService
    {
        private readonly IWebHostEnvironment _env;

        /// <summary>
        /// Recebe o ambiente web da aplicação por injeção de dependência.
        ///
        /// O IWebHostEnvironment permite acessar caminhos físicos da aplicação,
        /// como o WebRootPath, normalmente apontando para a pasta wwwroot.
        /// </summary>
        public LocalStorageService(IWebHostEnvironment env)
        {
            _env = env;
        }

        // ============================================================
        // 1. SALVAR ARQUIVO GENÉRICO
        // ============================================================

        /// <summary>
        /// Salva um arquivo no armazenamento local usando um caminho relativo.
        ///
        /// O método monta o caminho físico a partir do WebRootPath, cria a pasta
        /// caso ela ainda não exista e copia o conteúdo do arquivo enviado
        /// para o servidor.
        ///
        /// Ao final, retorna um caminho relativo padronizado com "/" para ser salvo
        /// no banco de dados e posteriormente usado para download ou exibição.
        /// </summary>
        public async Task<string> SalvarAsync(IFormFile arquivo, string caminhoRelativo)
        {
            var caminhoFisico = Path.Combine(_env.WebRootPath, caminhoRelativo);

            var pasta = Path.GetDirectoryName(caminhoFisico)!;
            Directory.CreateDirectory(pasta);

            using (var stream = new FileStream(caminhoFisico, FileMode.Create))
            {
                await arquivo.CopyToAsync(stream);
            }

            return "/" + caminhoRelativo.Replace("\\", "/");
        }

        // ============================================================
        // 2. VERIFICAR EXISTÊNCIA DO ARQUIVO
        // ============================================================

        /// <summary>
        /// Verifica se um arquivo existe no armazenamento local.
        ///
        /// O método recebe o caminho relativo salvo no banco, remove a barra inicial
        /// quando necessário e monta o caminho físico a partir do WebRootPath.
        /// </summary>
        public bool ArquivoExiste(string caminhoRelativo)
        {
            var caminhoFisico = Path.Combine(
                _env.WebRootPath,
                caminhoRelativo.TrimStart('/')
            );

            return File.Exists(caminhoFisico);
        }

        // ============================================================
        // 3. BAIXAR ARQUIVO
        // ============================================================

        /// <summary>
        /// Baixa um arquivo do armazenamento local.
        ///
        /// O método monta o caminho físico a partir do caminho relativo salvo no banco.
        /// Se o arquivo não existir, lança FileNotFoundException.
        /// Caso exista, retorna os bytes do arquivo para que o Controller possa
        /// devolver a resposta usando File().
        /// </summary>
        public async Task<byte[]> BaixarAsync(string caminhoRelativo)
        {
            var caminhoFisico = Path.Combine(
                _env.WebRootPath,
                caminhoRelativo.TrimStart('/')
            );

            if (!File.Exists(caminhoFisico))
                throw new FileNotFoundException();

            return await File.ReadAllBytesAsync(caminhoFisico);
        }

        // ============================================================
        // 4. SALVAR ENTREGA DE ATIVIDADE
        // ============================================================

        /// <summary>
        /// Salva o arquivo de uma entrega de atividade.
        ///
        /// Diferente do método genérico, este método organiza os arquivos
        /// de entregas em uma estrutura específica:
        /// uploads/entregas/{atividadeId}/{alunoId}
        ///
        /// Essa organização facilita localizar os arquivos enviados por aluno
        /// e por atividade.
        /// </summary>
        public async Task<string> SalvarEntrega(int atividadeId, int alunoId, IFormFile arquivo)
        {
            var pasta = Path.Combine(
                "wwwroot",
                "uploads",
                "entregas",
                atividadeId.ToString(),
                alunoId.ToString()
            );

            Directory.CreateDirectory(pasta);

            var caminho = Path.Combine(pasta, arquivo.FileName);

            using var stream = new FileStream(caminho, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            return caminho.Replace("wwwroot", "").Replace("\\", "/");
        }
    }
}